using Gateway.ServiceWorker;
using GpsTrackerMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureServices((hostContext, services) =>
                {
             
                    services.AddTransient<IGpsPakcetMQ>(a => new GpsPakcetMQ(a.GetService<IConfiguration>()["RabbitMQ"]));

                    services.AddHostedService<JT808_Worker>();
                    services.AddHostedService<BsjTcp_Worker>();
                    services.AddHostedService<BsjUdp_Worker>();

                    services.AddLogging(loggingBuilder =>
                                         {
                                             // configure Logging with NLog
                                             loggingBuilder.ClearProviders();
                                             loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                                             loggingBuilder.AddNLog();
                                         });
                });
    }
}
