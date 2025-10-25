
using Gateway.SockerServer.BSJ;
using GpsTrackerMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gateway.ServiceWorker
{
    class BsjTcp_Worker : BackgroundService
    {
        private readonly ILogger<BsjTcp_Worker> _logger;
        private readonly IConfiguration _config;
        private readonly GpsTrackerMQ.IGpsPakcetMQ _output;

        private BSJTcpServer _bsjServer = null;
        public BsjTcp_Worker(ILogger<BsjTcp_Worker> logger, IConfiguration config, IGpsPakcetMQ output)
        {
            _logger = logger;
            _config = config;
            _output = output;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            if (_bsjServer != null)
            {
                _bsjServer.Dispose();
                _bsjServer = null;
            }

            var port = int.Parse(_config["bsjPort"]);


            _bsjServer = new BSJTcpServer();
            _bsjServer.Start(new System.Net.IPEndPoint(IPAddress.Any, port));

            _bsjServer.OnSessionDataArrivalsEvent += bsjServer_OnSessionDataArrivalsEvent;

            _logger.LogInformation("bsjTcp监听端口：{0}", port);

            return base.StartAsync(cancellationToken);
        }



        private void bsjServer_OnSessionDataArrivalsEvent(BSJTcpSession session, byte[] data)
        {
            try
            {
                _output.SendPacket(new GPS_Packet_Model()
                {
                    DeviceType = 0,
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
            if (_bsjServer != null)
            {
                _bsjServer.Dispose();
                _bsjServer = null;
            }

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_bsjServer != null && _bsjServer.IsRunning)
                {
                    Console.WriteLine("BSJTcpServer 在线连接:{0}", _bsjServer.SessionCount);
                }
                await Task.Delay(1000 * 60, stoppingToken);
            }
        }
    }
}
