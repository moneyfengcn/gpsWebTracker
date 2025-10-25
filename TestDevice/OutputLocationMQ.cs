using GpsTrackerMQ;
using GpsTrackerMQ.Base;
using Microsoft.Extensions.Configuration;
using SaveDB.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestDevice
{
    public class OutputLocationMQ : GpsTrackerMQ.GpsLocationMQ
    {
        private readonly IStore _store;
        public OutputLocationMQ(IConfiguration config, IStore store) : base(config["RabbitMQ"], SubscribeType.生产者)
        {
            _store = store;
        }
        public override void SendGpsLocation(GPS_Location_Model obj)
        {
            obj.Time = new DateTime(obj.Time.Year,
                                    obj.Time.Month,
                                    obj.Time.Day,
                                    obj.Time.Hour,
                                    obj.Time.Minute,
                                    obj.Time.Second);
            _store.SaveGpsLocation(obj);
            base.SendGpsLocation(obj);
        }
    }
}
