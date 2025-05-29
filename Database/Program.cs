using System;
using System.Windows;
using Work_Test_Nordic_Next.Scripts;
using Database;
using Microsoft.Win32;
using DatabaseLib;
class Program
{
    [STAThread]
    static void Main()
    {
        if (OpenExplorer.Open("Select CSV File", "csv", out string fileName) == true)
        {
            string filePath = fileName;

            CSVReader.Read(filePath, out List<DatasetEntry> entriesList);
            if (DatabaseFactory.Create(entriesList, out SkuDatabase database))
            {
                var app = new Application();
                var window = new MainWindow();
                window.FillDatabase(database);
                app.Run(window);
            }
        }
    }
}