using System;
using System.Windows;
using Work_Test_Nordic_Next.Scripts;
using Database;
class Program
{
    [STAThread]
    static void Main()
    {
        CSVReader.Read("price_detail.csv", out List<DatasetEntry> entriesList);
        if(DatabaseFactory.Create(entriesList, out SkuDatabase database))
        {
            var app = new Application();
            var window = new MainWindow();
            window.FillDatabase(database);
            app.Run(window);
        }
    }
}