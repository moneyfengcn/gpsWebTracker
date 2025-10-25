using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTracker.DAL.Entitys
{
    public class GpsDevices : ISoftDelete
    {
        [Key]
        public string DeviceId { get; set; }
        public GpsTrackerUser OwnUser { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string Remarks { get; set; }


        //软删除
        public int IsDeleted { get; set; } = 0;
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
