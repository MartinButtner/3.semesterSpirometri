using LiveCharts.Wpf;
using LiveCharts;
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
using Spirometer.UI.Handlers;

namespace Spirometer.UI
{
    /// <summary>
    /// Interaction logic for SpirometerGraphWindow.xaml
    /// </summary>
    public partial class SpirometerGraphWindow : Window
    {
        private readonly SpirometerHandler _spirometerHandler;

        public SpirometerGraphWindow(SpirometerHandler spirometerHandler, string cpr)
        {
            InitializeComponent();
            _spirometerHandler = spirometerHandler ?? throw new ArgumentNullException(nameof(spirometerHandler));
            LoadGraphData(cpr);
        }

        private void LoadGraphData(string cpr)
        {
            var data = _spirometerHandler.GetByCPR(cpr);

            if (data == null || !data.Any())
            {
                MessageBox.Show("Ingen data fundet for det indtastede CPR-nummer.", "Ingen Data", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dates = data.Select(d => d.Date.ToShortDateString()).ToArray();
            var ratios = data.Select(d => d.Ratio).ToArray();
            var fev1Values = data.Select(d => d.FEV1).ToArray();
            var fvcValues = data.Select(d => d.FVC).ToArray();

            // Set up Ratio Chart
            RatioChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "FEV1/FVC Ratio",
                    Values = new ChartValues<double>(ratios),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 8
                }
            };

            RatioChart.AxisX.Clear();
            RatioChart.AxisX.Add(new Axis
            {
                Title = "Dato",
                Labels = dates,
                Foreground = System.Windows.Media.Brushes.Black,
                FontSize = 14
            });

            RatioChart.AxisY.Clear();
            RatioChart.AxisY.Add(new Axis
            {
                Title = "Ratio (%)",
                LabelFormatter = value => value.ToString("N0"),
                Foreground = System.Windows.Media.Brushes.Black,
                FontSize = 14
            });

            FEV1Chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "FEV1 (L)",
                    Values = new ChartValues<double>(fev1Values),
                    
                }
            };

            FEV1Chart.AxisX.Clear();
            FEV1Chart.AxisX.Add(new Axis
            {
                Title = "Dato",
                Labels = dates,
                FontSize = 14
            });

            FEV1Chart.AxisY.Clear();
            FEV1Chart.AxisY.Add(new Axis
            {
                Title = "FEV1 (L)",
                LabelFormatter = value => value.ToString("N1"),
                FontSize = 14
            });

            FVCChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "FVC (L)",
                    Values = new ChartValues<double>(fvcValues),
                   
                }
            };

            FVCChart.AxisX.Clear();
            FVCChart.AxisX.Add(new Axis
            {
                Title = "Dato",
                Labels = dates,
                FontSize = 14
            });

            FVCChart.AxisY.Clear();
            FVCChart.AxisY.Add(new Axis
            {
                Title = "FVC (L)",
                LabelFormatter = value => value.ToString("N1"),
                FontSize = 14
            });
        }
    }
}




