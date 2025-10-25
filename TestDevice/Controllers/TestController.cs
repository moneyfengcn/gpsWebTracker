using GpsTrackerMQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestDevice.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IGpsLocationMQ _locationMQ;

        public TestController(ILogger<TestController> logger, IGpsLocationMQ locationMQ)
        {
            _logger = logger;
            _locationMQ = locationMQ;
        }

        public IActionResult RecvGpsPosition(GpsTrackerMQ.GPS_Location_Model model)
        {
            _logger.LogInformation("接收：" + System.Text.Json.JsonSerializer.Serialize(model));
            _locationMQ.SendGpsLocation(model);
            return Ok();
        }
    }
}
