using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirometer.UI.Models
{
    public class Patient
    {
        [Key]
        public string CPRNumber { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }

        public List<SpirometerData> Data { get; set; } = new List<SpirometerData>();

    }
}
