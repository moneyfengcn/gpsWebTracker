
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Gateway
{
    public class JT808_Worker : BackgroundService
    {
        private readonly ILogger<JT808_Worker> _logger;
        private readonly IConfiguration _config;
        private readonly GpsTrackerMQ.IGpsPakcetMQ _output;

        private jt808.JT808Server jt808Server = null;
        public JT808_Worker(ILogger<JT808_Worker> logger, IConfiguration config, GpsTrackerMQ.IGpsPakcetMQ output)
        {
            _logger = logger;
            _config = config;
            _output = output;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            if (jt808Server != null)
            {
                jt808Server.Dispose();
                jt808Server = null;
            }

            var port = int.Parse(_config["jt808port"]);


            jt808Server = new jt808.JT808Server();
            jt808Server.Start(new System.Net.IPEndPoint(IPAddress.Any, port));

            jt808Server.OnSessionDataArrivalsEvent += Jt808Server_OnSessionDataArrivalsEvent;

            _logger.LogInformation("JT808监听端口：{0}", port);

            return base.StartAsync(cancellationToken);
        }

        private void Jt808Server_OnSessionDataArrivalsEvent(jt808.JT808Session session, byte[] data)
        {
            try
            {
                _output.SendPacket(new GpsTrackerMQ.GPS_Packet_Model()
                {
                    DeviceType = 1,
                    Packet = BitConverter.ToString(data)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (jt808Server != null)
            {
                jt808Server.Dispose();
                jt808Server = null;
            }

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (jt808Server != null && jt808Server.IsRunning)
                {
                    Console.WriteLine("jt808Server 在线连接:{0}", jt808Server.SessionCount);
                }
                await Task.Delay(1000 * 60, stoppingToken);
            }
        }
    }
}
