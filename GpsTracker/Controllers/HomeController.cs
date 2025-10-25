
using GpsTracker.Infrastructure;
using GpsTracker.Models;
using GpsTracker.Models.Home;
using GPSTracker.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.Controllers
{
    [TypeFilter(typeof(LogFilterAttribute))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<GpsTrackerUser> _userManager;
        private readonly SignInManager<GpsTrackerUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<GpsTrackerUser> userManager, SignInManager<GpsTrackerUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return View();
            }

            var Rememberme = model.Rememberme == "on";
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, Rememberme, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("登录", "登录失败 " + result.ToString());

                _logger.LogInformation("登录失败：UserName:{0}  Password:{1}  {2}", model.UserName, model.Password, result.ToString());
                return View();
            }
            _logger.LogInformation("登录成功：UserName:{0}  Password:{1}  IP:{2}", model.UserName, model.Password, this.Request.HttpContext.Connection.RemoteIpAddress.ToString());
            return RedirectToAction("RealMap", "GPS");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return View();
            }

            _logger.LogInformation("注册帐号:{0}   {1}", model.UserName, model.Password);

            //判断帐号是否己存在
            var tmp = await _userManager.FindByNameAsync(model.UserName);
            if (tmp != null)
            {
                ModelState.AddModelError("注册", "帐号己存在");
                return View();
            }
            //E-mail是否己注册
            tmp = await _userManager.FindByEmailAsync(model.Email);
            if (tmp != null)
            {
                ModelState.AddModelError("注册", "此Email地址己占用");
                return View();
            }

            //两次密码是否一样
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("注册", "两次密码不一致");
                return View();
            }

            var user = new GpsTrackerUser { UserName = model.UserName, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("注册", "创建帐号失败" + result.ToString());
                return View();
            }


            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("RealMap", "GPS");
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("注销登录:{0}", this.User.Identity.Name);
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<APIResult<int>> SaveNewPassword([FromBody] ChangePwdModel model)
        {
            if (!ModelState.IsValid)
            {
                return new APIResult<int>()
                {
                    State = false,
                    Msg = ModelState.Values.FirstOrDefault().Errors[0].ErrorMessage
                };
            }
            var user = await _userManager.FindByNameAsync(this.User.Identity.Name);

            if (user == null)
            {
                return new APIResult<int>()
                {
                    State = false,
                    Msg = "修改密码失败：用户不存在!"
                };
            }
            _logger.LogInformation("修改密码: {0}  {1}  {2}", this.User.Identity.Name, model.OldPassword, model.NewPassword);
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return new APIResult<int>()
                {
                    State = true,
                    Msg = "操作成功。"
                };
            }
            else
            {
                return new APIResult<int>()
                {
                    State = false,
                    Msg = "修改密码失败(" + result.ToString() + ")"
                };
            }
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
