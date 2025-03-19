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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Spirometer.UI
{
    /// <summary>
    /// Interaction logic for CriticalMeasurements.xaml
    /// </summary>
    public partial class CriticalMeasurements : Window
    {
        private readonly SpirometerHandler _spirometerHandler;

        public CriticalMeasurements(SpirometerHandler handler)
        {
            InitializeComponent();
            _spirometerHandler = handler;
            LoadCriticalMeasurements();
            
        }
        private void LoadCriticalMeasurements()
        {
            var criticalMeasurements = _spirometerHandler.GetCriticalMeasurements();
            CriticalDataGrid.ItemsSource = criticalMeasurements;
        }

        private void MarkAsSeen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int measurementId)
            {
                
                _spirometerHandler.MarkAsSeen(measurementId);

               
                var updatedMeasurements = _spirometerHandler.GetCriticalMeasurements();
                LoadCriticalMeasurements();
            }

        }
        private void CriticalDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CriticalDataGrid.SelectedItem is SpirometerData selectedData)
            {
                
                var recentMeasurements = _spirometerHandler.GetRecentMeasurements(selectedData.CPRNumber);

                DetailedMeasurementGrid.ItemsSource = recentMeasurements;
            }
        }

    }
}
