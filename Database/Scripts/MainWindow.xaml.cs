using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Database;

namespace Work_Test_Nordic_Next.Scripts
{
    public partial class MainWindow : Window
    {
        private Grid grid;

        public MainWindow()
        {
            InitializeComponent();
            CreateUI();
        }

        private void CreateUI()
        {
            grid = new Grid();
            this.Content = grid;
        }

        internal void FillDatabase(SkuDatabase database)
        {
            CollectionViewSource collectionView = new()
            {
                Source = database.GetAllValues()
            };

            DataGrid dataGrid = new DataGrid
            {
                AutoGenerateColumns = true,
                Margin = new Thickness(10),
                MinHeight = 300,
                CanUserAddRows = false,
                IsReadOnly = true
            };

            collectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SkuDatabase.Entry.CatalogEntryCode))); 
            dataGrid.ItemsSource = collectionView.View;

            grid.Children.Add(dataGrid);
        }
    }
}
