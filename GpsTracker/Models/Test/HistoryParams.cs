using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.Models.Test
{
    public class HistoryParams
    {
        public string DeviceId { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool FilterZero { get; set; }
    }
}
