using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace GpsTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                     .ConfigureWebHostDefaults(webBuilder =>
                     {
                         webBuilder.UseKestrel(option =>
                         {
                             option.Listen(System.Net.IPAddress.Any, 443, (lop) =>
                             {
                                 //参数为证书文件名称，证书密码
                                 lop.UseHttps("vip.pfx", "29hu727e06u20");
                             });
                             option.ListenAnyIP(80);
                         });
                         webBuilder.UseStartup<Startup>();
                     })
                     .ConfigureLogging(logging =>
                     {
                         logging.ClearProviders();
                         logging.SetMinimumLevel(LogLevel.Information);
                     })
                     .UseNLog();// NLog: Setup NLog for Dependency injection

        }
    }
}