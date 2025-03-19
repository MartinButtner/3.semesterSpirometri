using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spirometer.UI.Models;

namespace Spirometer.UI
{
    public class SpirometerContext : DbContext
    {
        public DbSet<Patient>  Patients { get; set; }
        public DbSet<SpirometerData> Data { get; set; }
        public string DbPath { get;}
        public SpirometerContext()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            if (basePath.Contains("bin"))
            {
                int i = basePath.IndexOf("bin");
                basePath = basePath.Substring(0, i);
            }
            DbPath = System.IO.Path.Join(basePath, "spirometer.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");

        }
    }
}
    