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

namespace LogisticDB
{
    /// <summary>
    /// Логика взаимодействия для ReportsWindow.xaml
    /// </summary>
    public partial class ReportsWindow : Window
    {
        LogisticData db;

        ReportsWindow()
        {
            InitializeComponent();
        }

        public static void ShowReportsDialog(LogisticData db)
        {
            var win = new ReportsWindow();
            win.db = db;
            win.ShowDialog();
        }

        private bool CheckInputDates()
        {
            if (FromCalendar.SelectedDate == null)
            {
                MessageBox.Show("Select from date!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (ToCalendar.SelectedDate == null)
            {
                MessageBox.Show("Select to date!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if(FromCalendar.SelectedDate >= ToCalendar.SelectedDate)
            {
                MessageBox.Show("From date must be lesser then to date!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void StayCoefButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInputDates())
                return;
            
            ReportDataGrid.ItemsSource = db.GetStayCoefReport(FromCalendar.SelectedDate.Value, ToCalendar.SelectedDate.Value);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
