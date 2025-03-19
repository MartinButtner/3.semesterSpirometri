using Spirometer.UI.Handlers;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Spirometer.UI
{
   
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialiser SpirometerContext og handlers
            var context = new SpirometerContext();
            var patientHandler = new PatientHandler(context);
            var spirometerHandler = new SpirometerHandler(context);

            // Opret MainWindow og injicer handlers
            var mainWindow = new MainWindow(patientHandler, spirometerHandler);
            mainWindow.Show();
        }
    }


}
