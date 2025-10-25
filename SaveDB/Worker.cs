using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaveDB
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly GpsTrackerMQ.IGpsPakcetMQ _pakcetMQ = null;
        private readonly Parser.IBSJParser _bSJParser;
        private readonly Parser.IJT808Parser _jt808Parser;

        public Worker(ILogger<Worker> logger, GpsTrackerMQ.IGpsPakcetMQ pakcetMQ, Parser.IBSJParser bSJParser, Parser.IJT808Parser jt808Parser)
        {
            _logger = logger;
            _pakcetMQ = pakcetMQ;

            _pakcetMQ.Register(new Func<GpsTrackerMQ.GPS_Packet_Model, bool>(On_Packet));
            _bSJParser = bSJParser;
            _jt808Parser = jt808Parser;
        }

        private bool On_Packet(GpsTrackerMQ.GPS_Packet_Model data)
        {
            try
            {
                _logger.LogInformation("接收MQ数据：{0}", data.Packet);
                var pack = ConvertPakcet(data.Packet);
                switch (data.DeviceType)
                {
                    case 0: //BSJ
                        _bSJParser.Parse(pack);
                        break;
                    case 1: //JT808
                        _jt808Parser.Parse(pack);
                        break;
                    default:
                        _logger.LogInformation("未知设备类型:{0}  {1}", data.DeviceType, data.Packet);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return true;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
               // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private byte[] ConvertPakcet(string packet)
        {
            var tmp = packet.Split('-', StringSplitOptions.RemoveEmptyEntries);
            var data = new byte[tmp.Length];

            for (int i = 0; i < tmp.Length; i++)
            {
                data[i] = Convert.ToByte(tmp[i], 16);
            }

            return data;
        }
    }
}
