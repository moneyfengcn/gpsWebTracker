
using GPSTracker.DAL;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.hubs
{
    public class GpsHubHelper : IGpsHubHelper
    {
        private Dictionary<string, string> _Connections = new Dictionary<string, string>();
        public readonly IServiceProvider _Services;
        private readonly IHubContext<gpsHub, IGpsEvents> _gpsHub;
        private readonly ILogger<GpsHubHelper> _logger;
        public GpsHubHelper(IHubContext<gpsHub, IGpsEvents> gpsHub, ILogger<GpsHubHelper> logger, IServiceProvider services)
        {
            _gpsHub = gpsHub;
            _logger = logger;
            _Services = services;
        }

        public void OnConnectedAsync(string userName, string connectionId)
        {
            lock (_Connections)
            {
                if (_Connections.ContainsKey(connectionId))
                {
                    _Connections[connectionId] = userName;
                }
                else
                {
                    _Connections.Add(connectionId, userName);
                }
            }
        }

        public void OnDisconnectedAsync(string connectionId)
        {
            lock (_Connections)
            {
                if (_Connections.ContainsKey(connectionId))
                {
                    _Connections.Remove(connectionId);
                }
            }
        }

        async public Task On_GpsLocation(string devceId, string name, double lat, double lng, float angle, float speed, DateTime time, string raw_pack)
        {
            await Task.Run(() =>
            {
                try
                {
                    using var scope = _Services.CreateScope();
                    using var _dbContext = scope.ServiceProvider.GetRequiredService<GpsTrackerContext>();
                    var dev = _dbContext.GpsDevices
                                        .AsNoTracking()
                                        .Include(a => a.OwnUser)
                                        .FirstOrDefault(a => a.DeviceId == devceId);
                    if (dev != null)
                    {
                        List<string> lstConnections = new List<string>();
                        lock (_Connections)
                        {
                            foreach (var item in _Connections)
                            {
                                if (item.Value == dev.OwnUser.UserName)
                                {
                                    lstConnections.Add(item.Key);
                                }
                            }
                        }
                        if (lstConnections.Count > 0)
                        {
                            _logger.LogInformation("推送定位：{0},{1},{2},{3},{4},{5}",
                                    dev.DeviceName,
                                    lat, lng,
                                    angle.ToString("0.0"),
                                    speed.ToString("0.0"),
                                    time);
                            foreach (var connectionId in lstConnections)
                            {
                                var output = _gpsHub.Clients.Client(connectionId);
                                output.On_GpsLocation(dev.DeviceId, dev.DeviceName, lat, lng, angle, speed, time, raw_pack);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            });
        }

        public Task Heartbeat(int value)
        {
            _logger.LogInformation("回复心跳");
            return Task.CompletedTask;
        }
    }
}
