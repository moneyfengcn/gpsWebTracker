using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.Models.Profile
{
    /// <summary>
    /// 地图默认视野
    /// </summary>
    public class DefaultMapViewModel
    {
        /// <summary>
        /// 缩放级别
        /// </summary>
        public int Zoom { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
