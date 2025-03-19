using System.Linq;
using Spirometer.UI.Models;

namespace Spirometer.UI.Handlers
{
    public class PatientHandler
    {
        private SpirometerContext _spirometerContext;
        public PatientHandler(SpirometerContext context)
        {
            _spirometerContext = context;
        }

        // Tilføj en ny patient
        public void Add(Patient patient)
        {
            _spirometerContext.Patients.Add(patient);
            _spirometerContext.SaveChanges();
        }

        // Hent en patient baseret på CPR-nummer
        public Patient GetByCpr(string cpr)
        {
            return _spirometerContext.Patients.FirstOrDefault(x => x.CPRNumber == cpr);
        }

        // Opdater en eksisterende patient
        public void Update(Patient patient)
        {
            _spirometerContext.Patients.Update(patient);
            _spirometerContext.SaveChanges();
        }
        
    }
}