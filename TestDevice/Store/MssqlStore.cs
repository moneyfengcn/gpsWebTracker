using Gps.DAL;
using GpsTrackerMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveDB.Store
{
    class MssqlStore : IStore
    {
        private GPSTracker.DAL.IDbContext _dbContext;

        public MssqlStore(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SaveGpsLocation(GPS_Location_Model model)
        {
            _dbContext.HistoryPosition.Insert(new Gps.DAL.Entitys.HistoryPosition()
            {
                DeviceId = model.DeviceId,
                Angle = (int)model.Angle,
                Lat = model.Lat,
                Lng = model.Lng,
                Speed = (int)Math.Ceiling(model.Speed),
                GpsTime = model.Time
            });
        }
    }
}
