using Spirometer.UI.Handlers;
using Spirometer.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiveCharts.Wpf;
using Spirometer.UI;

namespace SpirometerUI
{
    public partial class SpirometerMeasurements : Window
    {
        private TCP _tcpServer;
        private const double tolerance = 0.500;
        private bool isFirstMeasurementValid = false;
        private bool isSecondMeasurementValid = false;
        public double HighestFev1 { get; private set; }
        public double HighestFvc { get; private set; }
        public double HighestRatio { get; private set; }
        private double[]? flowData;

        private string cprNumber;
        private SpirometerHandler _spirometerHandler;

        public SpirometerMeasurements(string cpr)
        {
            InitializeComponent();
            cprNumber = cpr;
            CprTextBlock.Text = cpr;

            // Initialiser handler og TCP-server
            _spirometerHandler = new SpirometerHandler(new SpirometerContext());
            _tcpServer = new TCP();
            _tcpServer.OnDataReceived += ProcessMeasurementData;
            _tcpServer.OnStatusUpdated += UpdateStatusTextBox;
            _tcpServer.StartServer(1234);
        }

        private void UpdateStatusTextBox(string message)
        {
            Dispatcher.Invoke(() =>
            {
                StatusTextBox.Text += message;
            });
        }

        private void ProcessMeasurementData(string dataReceived)
        {
            Dispatcher.Invoke(() =>
            {
                StatusTextBox.Text += $"Modtaget data: {dataReceived}\n";
            });

            try
            {
                string[] dataParts = dataReceived.Split(',');
                double fev1 = Convert.ToDouble(dataParts[0].Split(':')[1]);
                double fvc = Convert.ToDouble(dataParts[1].Split(':')[1]);
                double ratio = Convert.ToDouble(dataParts[2].Split(':')[1]);
                string[] dataset = dataParts[3].Split(':')[1].Split(';');
                flowData = Array.ConvertAll(dataset, Double.Parse);

                var spirometry = new Algoritmer();
                (bool isValid, List<string> errors) = spirometry.ValidatePeak(flowData);

                if (!isFirstMeasurementValid)
                {
                    HandleFirstMeasurement(fev1, fvc, ratio, isValid, errors);
                }
                else if (!isSecondMeasurementValid)
                {
                    HandleSecondMeasurement(fev1, fvc, ratio, isValid, errors);
                }
                else
                {
                    HandleThirdMeasurement(fev1, fvc, ratio, isValid, errors);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    StatusTextBox.Text += $"Fejl ved behandling af data: {ex.Message}\n";
                });
            }
        }

        private void HandleFirstMeasurement(double fev1, double fvc, double ratio, bool isValid, List<string> errors)
        {
            if (isValid)
            {
                HighestFev1 = fev1;
                HighestFvc = fvc;
                HighestRatio = ratio;
                isFirstMeasurementValid = true;

                Dispatcher.Invoke(() =>
                {
                    StatusTextBox.Text += "Gyldig første måling gemt. Tag en ny måling.\n";
                    FirstMeasurementBox.Fill = new ImageBrush(new BitmapImage(new Uri("C:\\Users\\marti\\Desktop\\greencheckmark.png")));
                });
            }
            else
            {
                DisplayErrors(errors);
            }
        }

        private void HandleSecondMeasurement(double fev1, double fvc, double ratio, bool isValid, List<string> errors)
        {
            if (isValid && Math.Abs(fev1 - HighestFev1) <= tolerance && Math.Abs(fvc - HighestFvc) <= tolerance)
            {
                HighestFev1 = Math.Max(HighestFev1, fev1);
                HighestFvc = Math.Max(HighestFvc, fvc);
                HighestRatio = Math.Max(HighestRatio, ratio);
                isSecondMeasurementValid = true;

                Dispatcher.Invoke(() =>
                {
                    StatusTextBox.Text += "Gyldig anden måling gemt. Tag en ny måling.\n";
                    SecondMeasurementBox.Fill = new ImageBrush(new BitmapImage(new Uri("C:\\Users\\marti\\Desktop\\greencheckmark.png")));
                });
            }
            else
            {
                if (isValid)
                {
                    Dispatcher.Invoke(() =>
                    {
                        StatusTextBox.Text += "Målingen er udenfor tolerancen.\n";
                    });
                }

                DisplayErrors(errors);
            }
        }

        private void HandleThirdMeasurement(double fev1, double fvc, double ratio, bool isValid, List<string> errors)
        {
            if (isValid && Math.Abs(fev1 - HighestFev1) <= tolerance && Math.Abs(fvc - HighestFvc) <= tolerance)
            {
                HighestFev1 = Math.Max(HighestFev1, fev1);
                HighestFvc = Math.Max(HighestFvc, fvc);
                HighestRatio = Math.Max(HighestRatio, ratio);

                
                byte[] rawDataBlob = flowData != null ? flowData.SelectMany(BitConverter.GetBytes).ToArray() : Array.Empty<byte>();

                var spirometerData = new SpirometerData
                {
                    CPRNumber = cprNumber,
                    FEV1 = HighestFev1,
                    FVC = HighestFvc,
                    Ratio = HighestRatio,
                    Date = DateTime.Now,
                   RawData = rawDataBlob 
                };

                
                _spirometerHandler.Add(spirometerData);

                Dispatcher.Invoke(() =>
                {
                    StatusTextBox.Text += "Gyldig tredje måling godkendt og data gemt.\n";
                    ThirdMeasurementBox.Fill = new ImageBrush(new BitmapImage(new Uri("C:\\Users\\marti\\Desktop\\greencheckmark.png")));
                });

                ResetMeasurementState();
            }
            else
            {
                if (isValid)
                {
                    Dispatcher.Invoke(() =>
                    {
                        StatusTextBox.Text += "Målingen er udenfor tolerancen.\n";
                    });
                }

                DisplayErrors(errors);
            }
        }

        private void DisplayErrors(List<string> errors)
        {
            Dispatcher.Invoke(() =>
            {
                StatusTextBox.Text += "Ugyldig måling:\n";
                foreach (var error in errors)
                {
                    StatusTextBox.Text += error + "\n";
                }
                StatusTextBox.Text += "Prøv igen.\n";
            });
        }

        private void ResetMeasurementState()
        {
            isFirstMeasurementValid = false;
            isSecondMeasurementValid = false;
        }

        private void ShowGraphButton_Click(object sender, RoutedEventArgs e)
        {
            if (flowData == null || flowData.Length == 0)
            {
                StatusTextBox.Text += "Ingen rådata tilgængelige for at vise grafen.\n";
                return;
            }

            var plotWindow = new Window
            {
                Title = "Målingens Graf",
                Width = 800,
                Height = 600
            };

            var cartesianChart = new LiveCharts.Wpf.CartesianChart
            {
                AxisX = { new LiveCharts.Wpf.Axis { Title = "Tid (s)", Labels = GenerateTimeLabels(flowData.Length) } },
                AxisY = { new LiveCharts.Wpf.Axis { Title = "Flow (L/s)" } },
                Series = new LiveCharts.SeriesCollection
                {
                    new LiveCharts.Wpf.LineSeries
                    {
                        Title = "Flow-Data",
                        Values = new LiveCharts.ChartValues<double>(flowData),
                        PointGeometry = null
                    }
                }
            };

            plotWindow.Content = cartesianChart;
            plotWindow.Show();
        }

        private string[] GenerateTimeLabels(int dataLength)
        {
            string[] labels = new string[dataLength];
            for (int i = 0; i < dataLength; i++)
            {
                labels[i] = (i * 0.01).ToString("F2");
            }
            return labels;
        }

        private void ShowOldMeasurementsButton_Click(object sender, RoutedEventArgs e)
        {
            var measurements = _spirometerHandler.GetByCPR(cprNumber);

            if (measurements.Any())
            {
                var dataWindow = new Window
                {
                    Title = "Gamle Målinger",
                    Width = 600,
                    Height = 400,
                    Background = Brushes.LightBlue
                };

                var dataGrid = new DataGrid
                {
                    AutoGenerateColumns = false,
                    ItemsSource = measurements,
                    Margin = new Thickness(10)
                };

                dataGrid.Columns.Add(new DataGridTextColumn { Header = "CPR Nummer", Binding = new Binding("CPRNumber") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "FEV1", Binding = new Binding("FEV1") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "FVC", Binding = new Binding("FVC") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Ratio", Binding = new Binding("Ratio") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Dato", Binding = new Binding("Date") });

                dataWindow.Content = dataGrid;
                dataWindow.Show();
            }
            else
            {
                MessageBox.Show("Ingen gamle målinger fundet for dette CPR-nummer.", "Ingen Data", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AfslutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
