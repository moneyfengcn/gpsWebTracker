using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsTrackerMQ
{
    internal class MQDefinition
    {
        /// <summary>
        /// GPS报文
        /// </summary>
        public const string GPS_Packet = "GPS_Packet";
        /// <summary>
        /// 定位数据
        /// </summary>
        public const string GPS_Location = "GPS_Location";
    }

    public class GPS_Packet_Model
    {
        public int DeviceType { get; set; }
        public string Packet { get; set; }
    }

    public class GPS_Location_Model
    {
        public string DeviceId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public float Angle { get; set; }
        public float Speed { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        /// <summary>
        /// 原始数据包
        /// </summary>
        public string Pakcet { get; set; }
    }
}

