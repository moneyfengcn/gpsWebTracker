using GpsTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GpsTracker.Infrastructure.Razor;
using GpsTracker.Models.GPS;
using AutoMapper;
using GPSTracker.DAL;
using Microsoft.EntityFrameworkCore;
using GPSTracker.DAL.Entitys;
using Microsoft.AspNetCore.Identity;

namespace GpsTracker.Controllers
{
    [Authorize]
    public class GPSController : BaseController
    {
        private readonly ILogger<GPSController> _logger;
        private readonly GpsTrackerContext _dbContext;
        private readonly IMapper _mapper;
        public GPSController(ILogger<GPSController> logger, GpsTrackerContext dbContext, IMapper mapper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
        }
        //实时监控

        public IActionResult RealMap()
        {

            return View();
        }
        //历史轨迹
        public IActionResult History()
        {
            var list = _dbContext.GpsDevices
                .Include(a => a.OwnUser)
                .AsNoTracking()
                .Where(a => a.OwnUser.UserName == this.User.Identity.Name)
                .ToList();

            ViewBag.devList = list;

            return View();
        }
        //设备管理
        public async Task<IActionResult> Devices(Query<GpsDeviceModel, GpsDevicesParams> query, [FromServices] UserManager<GpsTrackerUser> userManager)
        {
            var user = await userManager.GetUserAsync(this.User);

            var qy = _dbContext.GpsDevices
                       .AsNoTracking()
                       .Where(a => a.OwnUser.UserName == this.User.Identity.Name)
                       .WhereIf(!string.IsNullOrEmpty(query.Params.DeviceId), a => a.DeviceId == query.Params.DeviceId)
                       .WhereIf(!string.IsNullOrEmpty(query.Params.DeviceName), a => a.DeviceName == query.Params.DeviceName)
                       .WhereIf(!string.IsNullOrEmpty(query.Params.DeviceType), a => a.DeviceType == query.Params.DeviceType)
                       ;

            var totalCount = await qy.CountAsync();

            var offset = (query.__pageIndex - 1) * query.__pageSize;

            var result = from dev in qy
                     join last in _dbContext.LastPositions
                     on dev.DeviceId equals last.Device.DeviceId
                     into temp
                     from leftjoin in temp.DefaultIfEmpty()
                     where dev.OwnUser.UserName == this.User.Identity.Name
                     orderby dev.CreateDate descending
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

            var list = await result.Skip(offset)
                        .Take(query.__pageSize)
                        .ToListAsync();


            query.ItemList = list;
            query.__totalRecord = totalCount;
            return PageView(query);
        }
        /// <summary>
        /// 实时监控引用的地图
        /// </summary>
        /// <returns></returns>
        public IActionResult Map()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AddNewDevice(string id)
        {
            GpsDevicesModel dev = null;
            var tmp = _dbContext.GpsDevices.FirstOrDefault(a => a.DeviceId == id && a.OwnUser.UserName == this.User.Identity.Name);
            if (tmp != null)
            {
                dev = _mapper.Map<GpsDevicesModel>(tmp);
            }
            return PartialView(dev);
        }

        [HttpPost]
        public async Task<APIResult<GpsDevicesModel>> AddNew_Device(GpsDevicesModel model, [FromServices] UserManager<GpsTrackerUser> userManager)
        {
            if (!ModelState.IsValid)
            {
                return new APIResult<GpsDevicesModel>()
                {
                    State = false,
                    Msg = GetModelErrorMessage()
                };
            }
            if (string.IsNullOrWhiteSpace(model.DeviceId))
            {
                return new APIResult<GpsDevicesModel>()
                {
                    State = false,
                    Msg = "设备序列号不能为空"
                };
            }

            var hasDev = _dbContext.GpsDevices.Any(a => a.DeviceId == model.DeviceId);
            if (hasDev)
            {
                return new APIResult<GpsDevicesModel>()
                {
                    State = false,
                    Msg = "此序列号设备己存在"
                };
            }
            _logger.LogInformation("添加设备 {0}  {1} {2}", this.User.Identity.Name, model.DeviceName, model.DeviceId);

            var user = await userManager.GetUserAsync(this.HttpContext.User);
            _dbContext.GpsDevices.Add(new GpsDevices()
            {
                DeviceId = model.DeviceId,
                DeviceName = model.DeviceName,
                DeviceType = model.DeviceType,
                OwnUser = user,
                Remarks = model.Remarks,
                CreateDate = DateTime.Now
            });

            await _dbContext.SaveChangesAsync();

            return new APIResult<GpsDevicesModel>()
            {
                State = true,
                Msg = "设备添加成功"
            };
        }

        [HttpPost]
        async public Task<APIResult<GpsDevicesModel>> Edit_Device(GpsDevicesModel model, [FromServices] UserManager<GpsTrackerUser> userManager)
        {
            if (!ModelState.IsValid)
            {
                return new APIResult<GpsDevicesModel>()
                {
                    State = false,
                    Msg = GetModelErrorMessage()
                };
            }
            if (string.IsNullOrWhiteSpace(model.DeviceId))
            {
                return new APIResult<GpsDevicesModel>()
                {
                    State = false,
                    Msg = "设备序列号不能为空"
                };
            }

            var dev = _dbContext.GpsDevices.FirstOrDefault(a => a.DeviceId == model.DeviceId);
            if (dev == null)
            {
                return new APIResult<GpsDevicesModel>()
                {
                    State = false,
                    Msg = "此序列号设备不存在"
                };
            }

            _logger.LogInformation("修改设备 {0}  {1} {2}", this.User.Identity.Name, dev.DeviceName, dev.DeviceId);
            var user = await userManager.GetUserAsync(this.User);
            dev.DeviceName = model.DeviceName;
            dev.DeviceType = model.DeviceType;
            dev.OwnUser = user;
            dev.Remarks = model.Remarks;

            _dbContext.GpsDevices.Update(dev);

            await _dbContext.SaveChangesAsync();

            return new APIResult<GpsDevicesModel>()
            {
                State = true,
                Msg = "设备修改成功"
            };
        }

        [HttpGet]
        public JsonResult DeleteDevice(string id)
        {

            var dev = _dbContext.GpsDevices.FirstOrDefault(a => a.DeviceId == id && a.OwnUser.UserName == this.User.Identity.Name);
            if (dev == null)
            {
                return new JsonResult(new
                {
                    state = 0,
                    msg = "该设备不存在"
                });
            }
            _logger.LogInformation("删除设备 {0}  {1} {2}", this.User.Identity.Name, dev.DeviceName, dev.DeviceId);

            _dbContext.GpsDevices.Remove(dev);
            _dbContext.SaveChanges();

            return new JsonResult(new
            {
                state = 1,
                msg = "操作成功"
            });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
