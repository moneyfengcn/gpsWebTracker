 
using GpsTrackerMQ.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.Infrastructure
{
    public class RecevieGpsLocationMQ : GpsTrackerMQ.GpsLocationMQ
    {
        public RecevieGpsLocationMQ(IConfiguration config) : base(config["RabbitMQ"], SubscribeType.消费者)
        {

        }
    }
}
