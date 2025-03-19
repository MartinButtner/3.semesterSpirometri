using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Spirometer.UI.Handlers;
using Spirometer.UI.Models;

namespace Spirometer.UI
{
    public partial class PatientRegistrationWindow : Window
    {
        private PatientHandler _patientHandler;

        public PatientRegistrationWindow(PatientHandler patientHandler)
        {
            InitializeComponent();
            _patientHandler = patientHandler ?? throw new ArgumentNullException(nameof(patientHandler));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CPRNumber.Focus();
        }
        private void CPRNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CPRNumber.Text.Length == 10) 
            {
                
                ScanMessage.Content = "CPR scannet korrekt! Indtast nu højde og vægt.";
                ScanMessage.Visibility = Visibility.Visible;

                
                PatientHeight.IsEnabled = true;
                PatientWeight.IsEnabled = true;

            
                ErrorMessage.Content = "";
                ErrorMessage.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Deaktiver felter, hvis CPR er ugyldigt
                ScanMessage.Visibility = Visibility.Collapsed;
                PatientHeight.IsEnabled = false;
                PatientWeight.IsEnabled = false;
            }
        }

        private void AddPatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CPRNumber.Text == string.Empty || PatientHeight.Text == string.Empty || PatientWeight.Text == string.Empty)
                {
                    ErrorMessage.Content = "Alle felter skal udfyldes.";
                    ErrorMessage.Visibility = Visibility.Visible;
                    AddPatientMessage.Visibility = Visibility.Collapsed;
                    return;
                }
                if (CPRNumber.Text.Length != 10 || !CPRNumber.Text.All(char.IsDigit))
                {
                    ErrorMessage.Content = "CPR-nummer skal være 10 cifre langt og kun indeholde tal.";
                    ErrorMessage.Visibility = Visibility.Visible;
                    AddPatientMessage.Visibility = Visibility.Collapsed;
                    return;
                }
                int height = Convert.ToInt32(PatientHeight.Text);
                if (height <=  0 || height >= 235)
                {
                    ErrorMessage.Content = "Patientens højde skal være mellem 1 og 234 cm.";
                    ErrorMessage.Visibility = Visibility.Visible;
                    AddPatientMessage.Visibility = Visibility.Collapsed;
                    return;
                }
                int weight = Convert.ToInt32(PatientWeight.Text);
                if (weight <= 9 || weight >= 300)
                {
                    ErrorMessage.Content = "Patientens vægt skal være mellem 10 og 299 kg.";
                    ErrorMessage.Visibility = Visibility.Visible;
                    AddPatientMessage.Visibility = Visibility.Collapsed;
                    return;
                }

                var existingPatient = _patientHandler.GetByCpr(CPRNumber.Text);
                if (existingPatient != null)
                {
                    // Opdater eksisterende patientdata
                    existingPatient.Height = height;
                    existingPatient.Weight = weight;
                    _patientHandler.Update(existingPatient);

                    AddPatientMessage.Content = "Eksisterende patientdata er blevet opdateret.";
                }
                else
                {
                    // Tilføj en ny patient, hvis CPR ikke findes
                    var newPatient = new Patient
                    {
                        CPRNumber = CPRNumber.Text,
                        Height = height,
                        Weight = weight,
                    };
                    _patientHandler.Add(newPatient);

                    AddPatientMessage.Content = "Ny patient er blevet tilføjet til databasen.";
                }

                AddPatientMessage.Visibility = Visibility.Visible;
                ErrorMessage.Visibility = Visibility.Collapsed;

                // Send CPR-nummeret videre til næste vindue
                VideoInstruction videoInstructionWindow = new VideoInstruction(CPRNumber.Text);
                videoInstructionWindow.Show();
                this.Close();
            }
            catch (FormatException)
            {
                ErrorMessage.Content = "Indtast venligst gyldige numeriske værdier for højde og vægt.";
                ErrorMessage.Visibility = Visibility.Visible;
                AddPatientMessage.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                ErrorMessage.Content = "Fejl: Kunne ikke tilføje eller opdatere patient. Prøv igen.";
                ErrorMessage.Visibility = Visibility.Visible;
                AddPatientMessage.Visibility = Visibility.Collapsed;
            }
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                if (textBox.Text == "Eks: 170" || textBox.Text == "Eks: 75")
                {
                    textBox.Text = "";
                    textBox.Foreground = Brushes.Black;
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    if (textBox.Name == "PatientHeight")
                    {
                        textBox.Text = "Eks: 170";
                    }
                    else if (textBox.Name == "PatientWeight")
                    {
                        textBox.Text = "Eks: 75";
                    }
                    textBox.Foreground = Brushes.Gray;
                }
            }
        }

    }
}
