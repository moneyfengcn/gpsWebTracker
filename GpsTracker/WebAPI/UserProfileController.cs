using GpsTracker.Models;
using GpsTracker.Models.Profile;
using GPSTracker.DAL;
using GPSTracker.DAL.Entitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.WebAPI
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly ILogger<UserProfileController> _logger;
        private readonly GpsTrackerContext _dbContext;

        public UserProfileController(ILogger<UserProfileController> logger, GpsTrackerContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 设置默认地图视野
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult<DefaultMapViewModel>> SaveMapDefaultView(DefaultMapViewModel model, [FromServices] UserManager<GpsTrackerUser> userManager)
        {
            var user = await userManager.GetUserAsync(this.User);
            var profile = _dbContext.UserProfiles.FirstOrDefault(a => a.UserId == user.Id);

            var json = System.Text.Json.JsonSerializer.Serialize(model);
            if (profile == null)
            {
                _dbContext.UserProfiles.Add(new UserProfile
                {
                    MapDefaultView = json,
                    UserId = this.User.Identity.Name
                });
            }
            else
            {
                profile.MapDefaultView = json;

                _dbContext.UserProfiles.Update(profile);
            }
            _dbContext.SaveChanges();

            return new APIResult<DefaultMapViewModel>()
            {
                State = true,
                Msg = "OK",
                Data = model
            };
        }

        async public Task<APIResult<DefaultMapViewModel>> GetMapDefaultView([FromServices] UserManager<GpsTrackerUser> userManager)
        {
            var user = await userManager.GetUserAsync(this.User);
            var userName = this.User.Identity.Name;
            var profile = _dbContext.UserProfiles.FirstOrDefault(a => a.UserId == user.Id);
            if (profile == null || string.IsNullOrEmpty(profile.MapDefaultView))
            {
                return new APIResult<DefaultMapViewModel>()
                {
                    State = false,
                    Msg = "没有数据",
                };
            }

            var tmp = System.Text.Json.JsonSerializer.Deserialize<DefaultMapViewModel>(profile.MapDefaultView);
            return new APIResult<DefaultMapViewModel>()
            {
                State = true,
                Msg = "OK",
                Data = tmp
            };

        }
    }
}
