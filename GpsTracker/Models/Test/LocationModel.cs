using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.Models.Test
{
    public class LocationModel
    {
        public string deviceId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public float Angle { get; set; }
        public float Speed { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;

    }
}
