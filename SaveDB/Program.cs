 
using GPSTracker.DAL;
using GpsTrackerMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace SaveDB
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
                    //services.AddTransient<GpsTrackerContext>(a => new GpsTrackerContext(a.GetService<IConfiguration>().GetConnectionString("GpsTrackerContextConnection"), a.GetService<ILogger<Gps.DAL.DbContext>>()));
                    services.AddEntityCore(hostContext.Configuration.GetConnectionString("GpsTrackerContextConnection"));
                    services.AddTransient<Store.IStore, Store.MssqlStore>();
                    services.AddTransient<IGpsPakcetMQ>(a => new GpsPakcetMQ(a.GetService<IConfiguration>()["RabbitMQ"]));
                    services.AddTransient<OutputLocationMQ>();

                    services.AddTransient<Parser.IBSJParser, Parser.BSJParser>();
                    services.AddTransient<Parser.IJT808Parser, Parser.JT808Parser>();
                    services.AddHostedService<Worker>();

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
