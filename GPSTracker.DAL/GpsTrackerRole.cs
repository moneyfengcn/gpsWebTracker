using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTracker.DAL
{
    public static class GpsTrackerUserRoles
    {
        public const string Administrator = "Administrator";
        public const string SuperAdministrator = "SuperAdministrator";
        public const string User = "User";
    }
    public class GpsTrackerRole : IdentityRole
    {

    }
}
