using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using Spirometer.UI.Handlers;
using Spirometer.UI.Models;

namespace Spirometer.UI
{


    public partial class DataWindow : Window
    {
        private readonly SpirometerHandler _spirometerHandler;

        public DataWindow(SpirometerHandler spirometerHandler)
        {
            InitializeComponent();
            _spirometerHandler = spirometerHandler;
        }


        private void AddSpirometer_Click(object sender, RoutedEventArgs e)
        {
            if (CPRNumberSpirometer.Text == string.Empty || FEV1.Text == string.Empty || FVC.Text == string.Empty ||
                Ratio.Text == string.Empty)
            {
                MessageBox.Show("Alle felter skal udfyldes.");
                return;
            }

            if (double.TryParse(FEV1.Text, out double fev1) &&
                double.TryParse(FVC.Text, out double fvc) &&
                double.TryParse(Ratio.Text, out double ratio))
            {

                byte[] rawData = Encoding.UTF8.GetBytes("Simuleret rådata fra spirometer");

                var spirometerData = new SpirometerData
                {
                    CPRNumber = CPRNumberSpirometer.Text,
                    FEV1 = Convert.ToDouble(FEV1.Text),
                    FVC = Convert.ToDouble(FVC.Text),
                    Ratio = Convert.ToDouble(Ratio.Text),
                    Date = DateTime.Now,
                    RawData = rawData
                };

                _spirometerHandler.Add(spirometerData);
            }
        }
        private void GetSpirometer_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                ErrorTextBlock.Visibility = Visibility.Collapsed;

                var cpr = CPRNumberSpirometer.Text;


                if (string.IsNullOrEmpty(cpr))
                {
                    ErrorTextBlock.Text = "Indtast venligst et CPR-nummer.";
                    ErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }


                var spirometerDataList = _spirometerHandler.GetByCPR(cpr);

                if (spirometerDataList != null && spirometerDataList.Any())
                {
                    SpirometerDataGrid.ItemsSource = spirometerDataList;
                }
                else
                {
                    ErrorTextBlock.Text = "Ingen målinger fundet for det indtastede CPR-nummer.";
                    ErrorTextBlock.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                // Vis fejl i TextBlock
                ErrorTextBlock.Text = $"En fejl opstod: {ex.Message}";
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void ShowGraph_Click(object sender, RoutedEventArgs e)
        {
            var cpr = CPRNumberSpirometer.Text;

            if (string.IsNullOrEmpty(cpr))
            {
                MessageBox.Show("Indtast venligst et CPR-nummer for at se grafen.");
                return;
            }

            var graphWindow = new SpirometerGraphWindow(_spirometerHandler, cpr);
            graphWindow.ShowDialog();
        }
    }
}
