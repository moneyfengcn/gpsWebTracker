using GpsTrackerMQ.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsTrackerMQ
{
    public class GpsPakcetMQ : Base.EventBus<GPS_Packet_Model>, IGpsPakcetMQ
    {
        public GpsPakcetMQ(string connectionString)
            : base(connectionString, MQDefinition.GPS_Packet, MQDefinition.GPS_Packet, MQ_WorkMode.顺序模式, SubscribeType.消费者)
        {
        }

        public void SendPacket(GPS_Packet_Model message)
        {
            base.SendMessage(message);
        }
    }

    public interface IGpsPakcetMQ : Base.IEventBus<GPS_Packet_Model>
    {
        void SendPacket(GPS_Packet_Model message);

    }
}
