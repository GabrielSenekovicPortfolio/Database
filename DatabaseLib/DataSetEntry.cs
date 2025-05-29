using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;

namespace Database
{
    public class DatasetEntry:IDatasetEntry
    {
        required public int PriceValueId { get; set; }
        required public DateTime Created { get; set; }
        required public DateTime Modified { get; set; }
        required public string CatalogEntryCode { get; set; }
        required public string MarketId { get; set; }
        required public string CurrencyCode { get; set; }
        required public DateTime ValidFrom { get; set; }
        required public DateTime? ValidUntil { get; set; }
        required public decimal UnitPrice { get; set; }

        public static void ModifyContext(CsvContext context)
        {
            context.TypeConverterOptionsCache.GetOptions<DateTime?>().NullValues.Add("NULL");
        }
    }
}
