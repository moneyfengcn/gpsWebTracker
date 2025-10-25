using GpsTrackerMQ.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsTrackerMQ
{
    public class GpsLocationMQ : Base.EventBus<GPS_Location_Model>, IGpsLocationMQ
    {
        public GpsLocationMQ(string connectionString, SubscribeType subscribeType)
            : base(connectionString, MQDefinition.GPS_Location, MQDefinition.GPS_Location, MQ_WorkMode.群发模式, subscribeType)
        {
        }

        virtual public void SendGpsLocation(GPS_Location_Model obj)
        {
            base.SendMessage(obj);
        }
    }

    public interface IGpsLocationMQ : Base.IEventBus<GPS_Location_Model>
    {
        void SendGpsLocation(GPS_Location_Model obj);
    }
}
