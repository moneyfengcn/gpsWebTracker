using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTracker.DAL
{

    /*
    * 程序包控制台执行以下命令 
    *       Add-Migration GpsTracker -o Migrations
    *           生成实体映射类
    *       update-database
    *           根据映射类更新到数据库    
    *           
    * 注意：
    *       1.在程序包控制台一定要将“默认项目：”设置为本项目
    *       2.将本项目设置为VS的启动项目
    *           
    */

    public class GpsTrackerContextFactory : IDesignTimeDbContextFactory<GpsTrackerContext>
    {
        public GpsTrackerContext CreateDbContext(string[] args)
        {
            var logger = LoggerFactory.Create(a => a.AddConsole())
            .CreateLogger<GpsTrackerContext>();

            var connStr = "Data Source=106.52.194.218;Initial Catalog=GpsTracker; User ID=sa; Password=qwe;";
            var optionsBuilder = new DbContextOptionsBuilder<GpsTrackerContext>();
            optionsBuilder.UseSqlServer(connStr);

            return new GpsTrackerContext(optionsBuilder.Options, logger);
        }
    }
}
