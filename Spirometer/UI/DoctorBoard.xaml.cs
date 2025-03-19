using Spirometer.UI.Handlers;
using Spirometer.UI.Models;
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

namespace Spirometer.UI
{
  

    public partial class DoctorBoard : Window
    {
        private readonly SpirometerHandler _spirometerHandler;

        public DoctorBoard(SpirometerHandler spirometerHandler)
        {
            InitializeComponent();
            _spirometerHandler = spirometerHandler;
        }

        private void ShowSpirometerData_Click(object sender, RoutedEventArgs e)
        {
            DataWindow DataWindow = new DataWindow(_spirometerHandler);
            DataWindow.ShowDialog();
        }

        private void ShowCriticalValues_Click(object sender, RoutedEventArgs e)
        {

            if (_spirometerHandler.GetCriticalMeasurements().Any())
            {
                var criticalWindow = new CriticalMeasurements(_spirometerHandler);
                criticalWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Ingen kritiske målinger fundet.");
            }
        }
    }
}

