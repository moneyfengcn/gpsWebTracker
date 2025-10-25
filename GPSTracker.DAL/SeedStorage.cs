using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTracker.DAL
{
    static internal class SeedStorage
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            //初始化 用户角色
            var userRole = new GpsTrackerRole
            {
                Name = GpsTrackerUserRoles.User,
                NormalizedName = GpsTrackerUserRoles.User.ToUpper(),
                Id = Guid.NewGuid().ToString()
            };
            var adminRole = new GpsTrackerRole
            {
                Name = GpsTrackerUserRoles.Administrator,
                NormalizedName = GpsTrackerUserRoles.Administrator.ToUpper(),
                Id = Guid.NewGuid().ToString()
            };
            var suRole = new GpsTrackerRole
            {
                Name = GpsTrackerUserRoles.SuperAdministrator,
                NormalizedName = GpsTrackerUserRoles.SuperAdministrator.ToUpper(),
                Id = Guid.NewGuid().ToString()
            };
            modelBuilder.Entity<GpsTrackerRole>().HasData(userRole, adminRole, suRole);

            //初始化默认管理员
            var hasher = new PasswordHasher<GpsTrackerUser>();
            var admin = new GpsTrackerUser()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "admin@USA.com",
                NormalizedEmail = "admin@USA.com".ToUpper(),
                UserName = "admin",
                RealName = "张学友",
                AvatarUrl = "/Images/Avatar/1.png",
                NormalizedUserName = "admin".ToUpper()
            };
            admin.PasswordHash = hasher.HashPassword(admin, "123123");

            //写入用户表
            modelBuilder.Entity<GpsTrackerUser>().HasData(admin);
            //建立角色关联
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = suRole.Id,
                UserId = admin.Id
            });


        }
    }
}
