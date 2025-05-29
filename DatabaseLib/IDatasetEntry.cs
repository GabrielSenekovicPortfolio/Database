using CsvHelper;

namespace Database
{
    public interface IDatasetEntry
    {
        public static abstract void ModifyContext(CsvContext context);
    }
}
