
using GPSTracker.DAL;
using GPSTracker.DAL.Entitys;
using GpsTrackerMQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveDB.Store
{
    class MssqlStore : IStore
    {

        public readonly IServiceProvider _Services;
        public MssqlStore(IServiceProvider service)
        {
            _Services = service;
        }

        public void SaveGpsLocation(GPS_Location_Model model)
        {
            using var scope = _Services.CreateScope();
            using var _dbContext = scope.ServiceProvider.GetRequiredService<GpsTrackerContext>();

            GpsDevices device = _dbContext.GpsDevices.FirstOrDefault(a => a.DeviceId == model.DeviceId);
            _dbContext.HistoryPositions.Add(new HistoryPosition()
            {
                Device = device,
                Angle = (int)model.Angle,
                Lat = model.Lat,
                Lng = model.Lng,
                Speed = (int)Math.Ceiling(model.Speed),
                GpsTime = model.Time
            });
            var last = _dbContext.LastPositions.FirstOrDefault(a => a.Device.DeviceId == device.DeviceId);
            if (last != null)
            { 
                last.Angle = (int)model.Angle;
                last.Lat = model.Lat;
                last.Lng = model.Lng;
                last.Speed = (int)Math.Ceiling(model.Speed);
                last.GpsTime = model.Time;

                _dbContext.LastPositions.Update(last);
            }
            else
            {
                last = new LastPosition()
                {
                    Device = device,
                    Angle = (int)model.Angle,
                    Lat = model.Lat,
                    Lng = model.Lng,
                    Speed = (int)Math.Ceiling(model.Speed),
                    GpsTime = model.Time                
                };
                _dbContext.LastPositions.Add(last);
            }
           
            _dbContext.SaveChanges();
        }
    }
}
