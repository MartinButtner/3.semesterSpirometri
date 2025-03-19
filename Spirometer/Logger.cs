using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Spirometer
{
    public class Logger
    {
        private static readonly string logFilePath = @"C:\Users\marti\Desktop\Tommappe6\Spirometer\bin\Debug\net8.0-windows\Log\access_log.txt";

        public static void LogDoctorAccess(string doctorId, string patientCpr, DateTime accesTime)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Læge {doctorId} tilgik patient CPR: {patientCpr}";

            try
            {
                
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kunne ikke skrive til logfilen: {ex.Message}");
               
            }
        }

    }
}
