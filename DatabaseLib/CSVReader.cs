using System;
using System.IO;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace Database
{
    public static class CSVReader
    {
        public static void Read<T>(string path, out List<T> entriesList) where T: IDatasetEntry
        {
            entriesList = new List<T>();
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                HeaderValidated = null,
                HasHeaderRecord = true
            };
            using (var sr = new StreamReader(path, Encoding.UTF8))
            using (var csv = new CsvReader(sr, csvConfig))
            {
                T.ModifyContext(csv.Context);

                try
                {
                    var entries = csv.GetRecords<T>();
                    entriesList = new List<T>(entries);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error parsing: " + ex.Message);
                }
            }
        }
    }
}
