
using GpsTracker.hubs;
using GpsTracker.Models;
using GpsTracker.Models.GPS;
using GpsTracker.Models.Test;
using GPSTracker.DAL;
using GPSTracker.DAL.Entitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IGpsHubHelper _gpsHubHelper;

        private readonly GpsTrackerContext _dbContext;

        public TestController(ILogger<TestController> logger, GpsTrackerContext dbContext, IGpsHubHelper gpsHubHelper)
        {
            _logger = logger;

            _dbContext = dbContext;
            _gpsHubHelper = gpsHubHelper;
        }


        /// <summary>
        /// 获取最后定位数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public JsonResult LastPosition()
        {
            //var query =
            //    from pos in _dbContext.LastPositions
            //    join dev in _dbContext.GpsDevices
            //    on pos.Device.DeviceId equals dev.DeviceId
            //    join user in _dbContext.Users
            //    on dev.OwnUser.UserName equals user.UserName
            //    where user.UserName == this.User.Identity.Name
            //    select
            //    pos;

            //var result = query.ToList();

            var result = _dbContext.LastPositions
                                    .AsNoTracking()
                                    .Include(a => a.Device)
                                    .Where(a => a.Device.OwnUser.UserName == this.User.Identity.Name)
                                    .ToList();



            return new JsonResult(result);
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public APIResult<List<HistoryPosition>> GetTestHistory()
        //{
        //    var startTime = new DateTime(2022, 9, 1);

        //    var result = _dbContext.HistoryPositions
        //                    .AsNoTracking()
        //                    .Include(a => a.Device)
        //                    //.Include(a => a.Device.OwnUser)
        //                    .Where(a => a.Device.DeviceId == "014538793164")
        //                    .Where(a => a.Speed > 0)
        //                    .Where(a => a.GpsTime >= startTime)                       
        //                    .OrderBy(a => a.GpsTime)
        //                    .ToList();

        //    return new APIResult<List<HistoryPosition>>()
        //    {
        //        State = true,
        //        Msg = "OK",
        //        Data = result
        //    };
        //}

        [Authorize]
        public APIResult<List<HistoryPosition>> GetHistory(HistoryParams query)
        {
            _logger.LogInformation("查询历史轨迹 {0}  {1} <-> {2}", query.DeviceId, query.BeginTime, query.EndTime);

            var dev = _dbContext.GpsDevices.FirstOrDefault(a => a.DeviceId == query.DeviceId && a.OwnUser.UserName == this.User.Identity.Name);
            if (dev == null)
            {
                return new APIResult<List<HistoryPosition>>()
                {
                    State = false,
                    Msg = "设备不存在"
                };
            }
            var result = _dbContext.HistoryPositions
                                .AsNoTracking()
                                .Include(a => a.Device)
                                .Include(a => a.Device.OwnUser)
                                .Where(a => a.Device.DeviceId == query.DeviceId)
                                .WhereIf(query.FilterZero, a => a.Speed > 0)
                                .WhereIf(query.BeginTime.HasValue, a => a.GpsTime >= query.BeginTime)
                                .WhereIf(query.EndTime.HasValue, a => a.GpsTime <= query.EndTime)
                                .OrderBy(a => a.GpsTime)
                                .ToList();


            return new APIResult<List<HistoryPosition>>()
            {
                State = true,
                Msg = "OK",
                Data = result
            };
        }


        /// <summary>
        /// 获取用户名下设备列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public JsonResult GetDevicesList()
        {
            var query = _dbContext.GpsDevices.AsNoTracking();

            var result = from dev in query
                         join last in _dbContext.LastPositions
                         on dev.DeviceId equals last.Device.DeviceId
                         into temp
                         from leftjoin in temp.DefaultIfEmpty()
                         where dev.OwnUser.UserName == this.User.Identity.Name
                         select new GpsDeviceModel
                         {
                             Remarks = dev.Remarks,
                             DeviceName = dev.DeviceName,
                             DeviceId = dev.DeviceId,
                             CreateDate = dev.CreateDate,
                             DeviceType = dev.DeviceType,
                             UserName = dev.OwnUser.UserName,
                             UserId = dev.OwnUser.Id,
                             GpsTime = leftjoin.GpsTime
                         };

            return new JsonResult(new
            {
                data = result.ToList()
            });
        }
    }
}
