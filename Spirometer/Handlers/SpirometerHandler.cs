using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spirometer.UI.Models;

namespace Spirometer.UI.Handlers
{
    public class SpirometerHandler
    {
        private SpirometerContext _spirometerContext;

        public SpirometerHandler(SpirometerContext context)
        {
            _spirometerContext = context;
        }
        
        public void Add(SpirometerData spirometerData)
        { 
            _spirometerContext.Add(spirometerData);
            _spirometerContext.SaveChanges();
            MarkCriticalMeasurements(spirometerData.CPRNumber);


        }

        public List<SpirometerData> GetByCPR(string cpr)
        {
            var spirometerData = _spirometerContext.Data;
            var data = spirometerData.Where(x => x.CPRNumber == cpr).OrderBy(x => x.Date).ToList();
           
            if (spirometerData == null || !spirometerData.Any())
            {
                throw new Exception($"Ingen målinger fundet for CPR-nummer: {cpr}");
            }

            Logger.LogDoctorAccess(LoginWindow.currentDoctorId, cpr, DateTime.Now);

            return data;
            
        }

        public void MarkCriticalMeasurements(string cpr)
        {
            var measurements = _spirometerContext.Data
                .Where(d => d.CPRNumber == cpr)
                .OrderByDescending(d => d.Date)
                .Take(2)
                .ToList();

            if (measurements.Count == 2)

            {
                var latest = measurements[0];
                var previous = measurements[1];

                double fev1Change = Math.Abs((latest.FEV1 - previous.FEV1) / previous.FEV1) * 100;
                double fvcChange = Math.Abs((latest.FVC - previous.FVC) / previous.FVC) * 100;

                if (fev1Change > 10 && fvcChange  > 10)
                {
                    latest.IsCritical = true;
                }
                else
                {
                    latest.IsCritical = false;
                }
                _spirometerContext.SaveChanges(); 
            }
        }
        public List<SpirometerData> GetCriticalMeasurements()
        {
            return _spirometerContext.Data
                .Where(d => d.IsCritical && !d.IsSeen) 
                .OrderBy(d => d.CPRNumber)
                .ThenBy(d => d.Date)
                .ToList();
        }
        public void MarkAsSeen(int measurementId)
        {
            var measurement = _spirometerContext.Data.FirstOrDefault(d => d.Id == measurementId);
            if (measurement != null)
            {
                measurement.IsSeen = true;
                _spirometerContext.SaveChanges();
            }
        }

        public List<SpirometerData> GetRecentMeasurements(string cpr)
        {
            return _spirometerContext.Data
                .Where(d => d.CPRNumber == cpr)
                .OrderByDescending(d => d.Date)
                .Take(2)
                .ToList();
        }

    }
}

