using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTracker.DAL
{
    static public class IdentityHostingStartup
    {
        //启用EntityFrameworkCore
        static public IServiceCollection AddEntityCore(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<GpsTrackerContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.EnableSensitiveDataLogging(false);                
            });
            return services;
        }

        //启用Asp.Net Core Identity
        static public IServiceCollection AddMyIdentityConfig(this IServiceCollection services)
        {
            services.AddDefaultIdentity<GpsTrackerUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;


                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 1;

                //// Lockout settings.
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                //options.Lockout.MaxFailedAccessAttempts = 5;
                //options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                options.User.RequireUniqueEmail = false;

                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddRoles<GpsTrackerRole>()
            .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<GpsTrackerUser, GpsTrackerRole>>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<GpsTrackerContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Home/AccessDenied";
                options.Cookie.Name = "OpenGPS";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(3);
                options.LoginPath = "/Home/Login";
                //   options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });
            return services;
        }
    }
}
