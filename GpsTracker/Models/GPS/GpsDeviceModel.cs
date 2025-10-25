using System;

namespace GpsTracker.Models.GPS
{
    public class GpsDeviceModel
    {
        public string Remarks { get; set; }
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public DateTime CreateDate { get; set; }
        public string DeviceType { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public DateTime? GpsTime { get; set; }
    }
}
