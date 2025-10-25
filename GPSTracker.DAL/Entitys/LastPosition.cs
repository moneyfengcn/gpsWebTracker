using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTracker.DAL.Entitys
{
    public class LastPosition: ISoftDelete
    {
        [Key]
        public long Id { get; set; }
        public GpsDevices Device { get; set; }         
        public double Lat { get; set; }
        
        public double Lng { get; set; }
     
        public int Angle { get; set; }
      
        public int Speed { get; set; }
     
        public DateTime GpsTime { get; set; }

        //软删除
        public int IsDeleted { get; set; } = 0;
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
