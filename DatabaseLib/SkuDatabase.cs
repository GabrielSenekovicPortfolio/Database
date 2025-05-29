using System.Data;

namespace Database
{
    public class SkuDatabase : Database<string, List<SkuDatabase.Entry>, SkuDatabase.Entry>
    {
        public class Entry : IDatabaseEntry<Entry>
        {
            public string CatalogEntryCode { get; set; }
            public string MarketId { get; set; }
            public decimal UnitPrice { get; set; }
            public string CurrencyCode { get; set; }
            public DateTime ValidFrom { get; set; }
            public DateTime? ValidUntil { get; set; }

            public Entry Copy() => new(this);
            public Entry(DatasetEntry dataset)
            {
                CatalogEntryCode = dataset.CatalogEntryCode;
                MarketId = dataset.MarketId;
                UnitPrice = dataset.UnitPrice;
                CurrencyCode = dataset.CurrencyCode;
                ValidFrom = dataset.ValidFrom;
                ValidUntil = dataset.ValidUntil;
            }
            public Entry(Entry entry)
            {
                CatalogEntryCode = entry.CatalogEntryCode;
                MarketId = entry.MarketId;
                CurrencyCode = entry.CurrencyCode;
                ValidFrom = entry.ValidFrom;
                ValidUntil = entry.ValidUntil;
                UnitPrice = entry.UnitPrice;
            }

            public DateTime StartDate() => ValidFrom;

            public DateTime? EndDate() => ValidUntil;

            public void SetStart(DateTime start)
            {
                ValidFrom = start;
            }

            public void SetEnd(DateTime? end)
            {
                ValidUntil = end;
            }
        }
        public SkuDatabase(Dictionary<string, List<Entry>> values) : base(values){ }
    }
}
