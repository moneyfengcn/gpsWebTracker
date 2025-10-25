using GPSTracker.DAL.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveDB.Store
{
    public interface IStore
    {
        void SaveGpsLocation(GpsTrackerMQ.GPS_Location_Model location);
    }
}
