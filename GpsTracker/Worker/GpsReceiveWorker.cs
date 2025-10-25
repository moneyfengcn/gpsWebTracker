using GpsTracker.hubs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GpsTracker.Worker
{
    public class GpsReceiveWorker : BackgroundService
    {
        private readonly ILogger<GpsReceiveWorker> _logger;
        private readonly GpsTrackerMQ.IGpsLocationMQ _gpsLocationMQ = null;
        private readonly IGpsHubHelper _gpsHubHelper;

        public GpsReceiveWorker(ILogger<GpsReceiveWorker> logger, GpsTrackerMQ.IGpsLocationMQ gpsLocationMQ, IGpsHubHelper gpsHubHelper)
        {
            _logger = logger;
            _gpsLocationMQ = gpsLocationMQ;

            _gpsLocationMQ.Register(new Func<GpsTrackerMQ.GPS_Location_Model, bool>(On_Packet));
            _gpsHubHelper = gpsHubHelper;
        }

        private bool On_Packet(GpsTrackerMQ.GPS_Location_Model data)
        {
            try
            {
                _logger.LogInformation("websocket推送:{0}", System.Text.Json.JsonSerializer.Serialize(data));
                _gpsHubHelper.On_GpsLocation(data.DeviceId, "", data.Lat, data.Lng, data.Angle, data.Speed, data.Time, data.Pakcet);
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
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
