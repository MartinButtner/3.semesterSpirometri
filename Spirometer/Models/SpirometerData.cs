using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Spirometer.UI.Models
{
    public class SpirometerData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double FEV1 { get; set; } 
        public double FVC { get; set; }
        public double Ratio { get; set; }
        public DateTime Date { get; set; }
        public bool IsCritical { get; set; } = false;
        public bool IsSeen { get; set; } = false;

       public byte[]? RawData { get; set; }

        [ForeignKey("Patient")]
        public string? CPRNumber { get; set; }
        public Patient Patient { get; set; } = null!;


        


    }
}
