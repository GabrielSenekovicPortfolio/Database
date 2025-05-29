using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static Database.SkuDatabase;

namespace Database
{
    public class DatabaseFactory
    {
        public static bool Create(List<DatasetEntry> datasets, out SkuDatabase database)
        {
            database = null;
            if(FillDatabase(datasets, out var values))
            {
                database = new SkuDatabase(values);
            }
            return database != null;
        }
        static bool FillDatabase(List<DatasetEntry> datasets, out Dictionary<string, List<Entry>> values)
        {
            values = new();
            if (datasets == null || datasets.Count == 0) return false;
            foreach (var dataset in datasets)
            {
                Entry entry = new(dataset);
                if (values.ContainsKey(dataset.CatalogEntryCode))
                {
                    AddToExtantEntry(values, entry);
                }
                else
                {
                    AddNewEntry(values, entry);
                }
            }
            for (int i = 0; i < values.Count; i++)
            {
                values[values.ElementAt(i).Key] = values.ElementAt(i).Value.OrderBy(v => v.MarketId).ThenBy(v => v.CurrencyCode).ThenBy(v => v.ValidFrom).ToList();
            }
            Trim(values);
            return Validate(values);
        }
        static void Trim(Dictionary<string, List<Entry>> values)
        {
            //Last trim to check 
            if (values.Count == 0) return;

            foreach (var kvp in values)
            {
                var entries = kvp.Value;

                var grouped = entries.GroupBy(e => new { e.MarketId, e.CurrencyCode });

                foreach (var group in grouped)
                {
                    var list = group.ToList();
                    for (int i = 0; i < list.Count() - 1; i++)
                    {
                        var current = list[i];
                        var next = list[i + 1];

                        if (current.UnitPrice == next.UnitPrice)
                        {
                            //This only catches entries that initially overlap. Check trim later
                            values[current.CatalogEntryCode].Remove(current);
                            DateTime earlierStart = DateTime.Compare(current.StartDate(), next.StartDate()) < 0 ? current.StartDate() : next.StartDate();
                            DateTime? entryEnd_1 = current.EndDate();
                            DateTime? entryEnd_2 = next.EndDate();
                            DateTime? laterEnd = entryEnd_1 == null ? entryEnd_1 : entryEnd_2 == null ? entryEnd_2 :
                                DateTime.Compare(entryEnd_1.Value, entryEnd_2.Value) > 0 ? entryEnd_1.Value : entryEnd_2.Value;
                            next.SetStart(earlierStart);
                            next.SetEnd(laterEnd);
                        }
                    }
                }
            }
        }
        static bool Validate(Dictionary<string, List<Entry>> values)
        {
            if (values.Count == 0) return false;

            foreach (var kvp in values)
            {
                var entries = kvp.Value;

                var grouped = entries.GroupBy(e => new { e.MarketId, e.CurrencyCode });

                foreach (var group in grouped)
                {
                    var list = group.ToList();
                    if(!DateTimeValidator.Validate(list, (Entry current, string errorMessage) =>
                    {
                        Console.WriteLine($"Validation failed: {current.CatalogEntryCode} ({current.MarketId}/{current.CurrencyCode})" + errorMessage);
                    }))
                    {
                        return false;
                    }
                    for (int i = 0; i < list.Count() - 1; i++) 
                        //Now we go through the list THRICE. Which isn't great, but let's get this to work before we worry about time optimisation
                    {
                        var current = list[i];
                        var next = list[i + 1];

                        if(current.UnitPrice == next.UnitPrice)
                        {
                            Console.WriteLine($"Validation failed: {current.CatalogEntryCode} ({current.MarketId}/{current.CurrencyCode}) has the same price twice!");
                            //return false;
                        }
                    }
                }
            }
            return true;
        }
        static void AddToExtantEntry(Dictionary<string, List<Entry>> values, Entry dataset)
        {
            if (GetDuplicates(values, dataset, out List<Entry> duplicates))
            {
                HandleDuplicate(values, dataset, duplicates);
            }
            else
            {
                //This entry has a new market or a new currency
                values[dataset.CatalogEntryCode].Add(dataset);
            }
        }
        static bool GetDuplicates(Dictionary<string, List<Entry>> values, Entry dataset, out List<Entry> duplicates)
        {
            duplicates = values[dataset.CatalogEntryCode].Where(
                v => v.MarketId == dataset.MarketId && //Same market
                v.CurrencyCode == dataset.CurrencyCode && //Same currency
                !DateTimeSorter.Overlaps(v, dataset)).ToList();
            return duplicates != null && duplicates.Count > 0;
        }
        static void HandleDuplicate(Dictionary<string, List<Entry>> values, Entry dataset, List<Entry> duplicates)
        {
            //If this entry matches the market and the currency of an extant entry, then we must compare prices and validity dates
            //If the entry period starts and ends within another entry, then that entry must be split into two IF it is less cheap
            //If the entry period starts within one entry, and ends in another entry, then it might be cut off depending on which entry is cheaper
            //Entries that have a validUntil that are null will be active whenever there are no other prices
            Entry newEntry;
            for (int i = 0; i < duplicates.Count; i++)
            {
                var entry = duplicates[i];

                if(CheckOverlappingData(values, entry, dataset) == ReturnValue.FAILURE)
                {
                    return;
                } 
                CheckOverlappingData(values, dataset, entry); 

                if(entry.ValidUntil.HasValue && entry.ValidFrom == entry.ValidUntil.Value)
                {
                    values[entry.CatalogEntryCode].Remove(entry);
                }
                if(entry.UnitPrice == dataset.UnitPrice)
                {
                    //This only catches entries that initially overlap. Check trim later
                    values[entry.CatalogEntryCode].Remove(entry);
                    DateTime earlierStart = DateTime.Compare(entry.StartDate(), dataset.StartDate()) < 0 ? entry.StartDate() : dataset.StartDate();
                    DateTime? entryEnd_1 = entry.EndDate();
                    DateTime? entryEnd_2 = dataset.EndDate();
                    DateTime? laterEnd = entryEnd_1 == null ? entryEnd_1 : entryEnd_2 == null ? entryEnd_2 :
                        DateTime.Compare(entryEnd_1.Value, entryEnd_2.Value) > 0 ? entryEnd_1.Value : entryEnd_2.Value;
                    dataset.SetStart(earlierStart);
                    dataset.SetEnd(laterEnd);
                }
            }
            if (dataset.ValidUntil.HasValue && dataset.ValidFrom == dataset.ValidUntil.Value) return;

            newEntry = new Entry(dataset);
            values[dataset.CatalogEntryCode].Add(newEntry);
        }
        static ReturnValue CheckOverlappingData(Dictionary<string, List<Entry>> values, Entry a, Entry b)
        {
            return DateTimeSorter.HandleOverlap(values, a, b,
                (x, y) => x.UnitPrice < y.UnitPrice,
                (e) => values[a.CatalogEntryCode].Add(e),
                (e) => values[b.CatalogEntryCode].Remove(e));
        }
        
        static void AddNewEntry(Dictionary<string, List<Entry>> values, Entry entry)
        {
            values.Add(entry.CatalogEntryCode, new());
            values[entry.CatalogEntryCode].Add(entry);
        }
    }
}