using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.Models.Home
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Rememberme { get; set; }
    }
}
