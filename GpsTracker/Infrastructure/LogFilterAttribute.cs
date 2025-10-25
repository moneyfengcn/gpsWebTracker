using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GpsTracker.Infrastructure
{
    public class LogFilterAttribute : Attribute, IAsyncActionFilter
    {
        private readonly ILogger<LogFilterAttribute> _logger;

        public LogFilterAttribute(ILogger<LogFilterAttribute> logger)
        {
            _logger = logger;
        }

        async public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var ip = context.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                var path = context.HttpContext.Request.Path;
                SaveLocationInfo(ip, path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            await next();
        }

        void SaveLocationInfo(string ip, string path)
        {
            //var location = await GetIPInfo(ip);
            //_logger.LogInformation("访客IP:\t{0}\t{1}\t\t\t目标页面:{2}", ip, location, path);
        }

        async private Task<string> GetIPInfo(string ip)
        {
            const string API_Address = "https://www.36ip.cn/?ip={0}";

            using var http = new HttpClient();

            var json = await http.GetStringAsync(string.Format(API_Address, ip));

            return json;

        }


        public class IPLocation
        {
            public string status { get; set; }
            public string info { get; set; }
            public string infocode { get; set; }
            public string province { get; set; }
            public string city { get; set; }
            public string adcode { get; set; }
            public string rectangle { get; set; }
        }

    }
}
