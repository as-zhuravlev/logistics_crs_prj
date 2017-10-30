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
using System.Windows.Shapes;
using Npgsql;

namespace LogisticDB
{
    /// <summary>
    /// Логика взаимодействия для BuyCarWindow.xaml
    /// </summary>
    public partial class BuyCarWindow : Window
    {
        LogisticData db;
        BuyCarWindow()
        {
            InitializeComponent();
        }

        public static bool ShowBuyDialog(LogisticData db)
        {
            var win = new BuyCarWindow();
            win.db = db;
            win.CitiesComboBox.ItemsSource = db.GetCities();
            win.ModelsListView.ItemsSource = db.GetCarModelCargoTypes();
            win.ShowDialog();
            return win.DialogResult ?? false;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (CitiesComboBox.SelectedItem == null)
            {
                MessageBox.Show("Select city!", "", MessageBoxButton.OK, MessageBoxImage.Error); 
                return;
            }
            if (ModelsListView.SelectedItem == null)
            {
                MessageBox.Show("Select model!", "", MessageBoxButton.OK, MessageBoxImage.Error); 
                return;
            }
            if(string.IsNullOrWhiteSpace(NumberTextBox.Text)) 
            {
                MessageBox.Show("Input number!", "", MessageBoxButton.OK, MessageBoxImage.Error); 
                return;
            }
            if ((DateTime)DateCalender.SelectedDate == null)
            {
                MessageBox.Show("Select date!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                db.BuyCar((CitiesComboBox.SelectedItem as City).id,
                        (ModelsListView.SelectedItem as CarModelCargoType).id,
                        (DateTime)DateCalender.SelectedDate, NumberTextBox.Text);
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(String.Format("{0}\r\nHint: {1}", ex.Message, ex.Hint), "", 
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
