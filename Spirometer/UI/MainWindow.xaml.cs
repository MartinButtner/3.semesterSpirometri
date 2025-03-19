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
using Spirometer.UI.Handlers;
using Spirometer.UI.Models;

namespace Spirometer.UI
{
    public partial class MainWindow : Window
    {
        private readonly PatientHandler _patientHandler;
        private readonly SpirometerHandler _spirometerHandler;

        public MainWindow(PatientHandler patientHandler, SpirometerHandler spirometerHandler)
        {
            InitializeComponent();
            _patientHandler = patientHandler;
            _spirometerHandler = spirometerHandler;
        }

        private void OpenPatientRegistration_Click(object sender, RoutedEventArgs e)
        {
            var patientWindow = new PatientRegistrationWindow(_patientHandler);
            patientWindow.Show();
        }

        private void OpenDoctorLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();

            if (loginWindow.IsLoggedIn)
            {
                var doctorDashboard = new DoctorBoard(_spirometerHandler);
                doctorDashboard.Show();
            }
        }
    }
}

