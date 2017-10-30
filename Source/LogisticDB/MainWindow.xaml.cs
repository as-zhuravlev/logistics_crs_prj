using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogisticDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LogisticData db;
        
        public MainWindow()
        {
            InitializeComponent();
            db = new LogisticData();
            CarsListView.ItemsSource = db.GetCarViews();
        }
        
        private void BuyCarButton_Click(object sender, RoutedEventArgs e)
        {

            if (BuyCarWindow.ShowBuyDialog(db))
            {
                var cars = db.GetCarViews().ToList();
                CarsListView.ItemsSource = cars;
                CarsListView.SelectedItem = cars.Aggregate((a, b) => (a.id > b.id ? a : b));
            }
        }

        private void MakeOrderButton_Click(object sender, RoutedEventArgs e)
        {
            int car_id = -1;
            if (MakeOrderWindow.ShowMakeOrderDialog(db, out car_id))
            {
                var cars = CarsListView.ItemsSource as IEnumerable<CarView>;
                if (cars == null)
                    return;
                CarsListView.SelectedItem = null;
                CarsListView.SelectedItem = cars.FirstOrDefault(x => x.id == car_id);
            }
        }

        private void SellCarButton_Click(object sender, RoutedEventArgs e)
        {
            SellCarWindow.ShowSellCarDialog(db);
            CarsListView.ItemsSource = db.GetCarViews();

        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            ReportsWindow.ShowReportsDialog(db);
        }

        private void CarTransactionsListView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var car = CarsListView.SelectedItem as Car;
            if (car == null)
                return;
            var tr = db.GetCarTransactions(car).ToList();
            CarTransactionsListView.ItemsSource = tr;
        }
    }
}








