using System;
using System.Windows;
using SpirometerUI;

namespace Spirometer.UI
{
    public partial class InstructionText : Window
    {
        private int _currentStepIndex = 0;
        private readonly string _cprNumber;
        private double _highestFev1, _highestFvc, _highestRatio;

        public InstructionText(string cprNumber)
        {
            InitializeComponent();
            _cprNumber = cprNumber; 
            ShowCurrentSteps();
        }

        private void ShowCurrentSteps()
        {
            
            StepGroup0.Visibility = Visibility.Collapsed;
            StepGroup1.Visibility = Visibility.Collapsed;
          
            ResultDisplay.Visibility = Visibility.Collapsed;

            switch (_currentStepIndex)
            {
                case 0:
                    StepGroup0.Visibility = Visibility.Visible;
                    BackButton.Visibility = Visibility.Collapsed;
                    Instructiontext.Text = "Hvis instrukser er læst og forstået, tryk på næste for at fortsætte.";
                    break;

                case 1:
                    StepGroup1.Visibility = Visibility.Visible;
                    Instructiontext.Text = "Hvis instrukser er læst og forstået, tryk på næste for at fortsætte.";
                    BackButton.Visibility = Visibility.Visible;
                    break;

                case 2:
                    
                    var spirometerPage = new SpirometerMeasurements(_cprNumber);
                    spirometerPage.ShowDialog(); 

                   
                    _highestFev1 = spirometerPage.HighestFev1;
                    _highestFvc = spirometerPage.HighestFvc;
                    _highestRatio = spirometerPage.HighestRatio;

                   
                    ResultDisplay.Visibility = Visibility.Visible;
                    Fev1Result.Text = $"Højeste FEV1: {_highestFev1}";
                    FvcResult.Text = $"Højeste FVC: {_highestFvc}";
                    RatioResult.Text = $"Højeste Ratio: {_highestRatio}";

                    Instructiontext.Text = "Målingen er nu færdig.";
                    BackButton.Visibility = Visibility.Collapsed;
                    NextButton.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            _currentStepIndex++;

            if (_currentStepIndex <= 3)
            {
                ShowCurrentSteps();
            }
            else
            {
                MessageBox.Show("Instruktionen er færdig.");
                this.Close();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStepIndex > 0)
            {
                _currentStepIndex--;
                ShowCurrentSteps();
            }
        }
    }
}
