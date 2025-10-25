using GPSTracker.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.hubs
{
    [Authorize]
    public class gpsHub : Hub<IGpsEvents>
    {
        private ILogger<gpsHub> _logger;
        private readonly IGpsHubHelper _gpsHubHelper;
        public gpsHub(ILogger<gpsHub> logger, IGpsHubHelper gpsHubHelper)
        {
            _logger = logger; 
            _gpsHubHelper = gpsHubHelper;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("websocket连接进入:{0}  {1}", this.Context.User.Identity.Name, this.Context.ConnectionId);

            _gpsHubHelper.OnConnectedAsync(this.Context.User.Identity.Name, this.Context.ConnectionId);
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation("websocket连接退出:{0}  {1}", this.Context.User.Identity.Name, this.Context.ConnectionId);

            _gpsHubHelper.OnDisconnectedAsync(this.Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }



        //public async Task On_GpsLocation(string devceId, double lat, double lng, float angle, float speed, DateTime time)
        //{


        //    var dev = _dbContext.GpsDevices.GetSingle(a => a.DeviceId == devceId && a.OwnUser == this.Context.User.Identity.Name);
        //    if (dev != null)
        //    {

        //        await Clients.Client(this.Context.ConnectionId).On_GpsLocation(dev.DeviceName, lat, lng, angle, speed, time);
        //    }
        //}

        public Task Heartbeat(int value)
        {
            _logger.LogInformation("websocket心跳:{0}  {1}", this.Context.User.Identity.Name, value);

            Clients.Clients(this.Context.ConnectionId).Heartbeat(value);
            return Task.CompletedTask;
        }
    }


    public interface IGpsEvents
    {
        Task On_GpsLocation(string devceId, string name, double lat, double lng, float angle, float speed, DateTime time, string raw_pack);
        Task Heartbeat(int value);
    }
}
