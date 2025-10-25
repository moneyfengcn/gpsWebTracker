
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GpsTracker.hubs;
using GpsTrackerMQ;
using GpsTracker.Infrastructure;
using GPSTracker.DAL;

namespace GpsTracker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {   
            //开启压缩
            services.AddResponseCompression();
            //允许跨域
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.AllowAnyHeader();
                        policy.AllowAnyMethod();
                        policy.AllowAnyOrigin();
                       // policy.AllowCredentials();
                    });
            });


            services.AddControllersWithViews();
        
            services.AddSignalR(option =>
            {
                //表示客户端如果在3min内没有向服务器发送任何消息，那么服务器端则会认为客户端已经断开连接了
                option.ClientTimeoutInterval = TimeSpan.FromMinutes(3);
                //表示如果服务器未在15s内向客户端发送消息，在15s的时候服务器会自动ping客户端，是连接保持打开的状态。
                //option.KeepAliveInterval = TimeSpan.FromSeconds(60);
            });
            services.AddAutoMapper(typeof(Program).Assembly);
            //services.AddTransient<IDbContext>(a => new Gps.DAL.DbContext(Configuration.GetConnectionString("GpsTrackerContextConnection"), a.GetService<ILogger<Gps.DAL.DbContext>>()));
            services.AddEntityCore(Configuration.GetConnectionString("GpsTrackerContextConnection"));
            services.AddMyIdentityConfig();

#if !false
            //一定要单例
            services.AddSingleton<IGpsHubHelper, GpsHubHelper>();
            //消息队列
            services.AddSingleton<IGpsLocationMQ, RecevieGpsLocationMQ>();
            //接收服务
            services.AddHostedService<Worker.GpsReceiveWorker>();
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/AccessDenied");
            }
            app.UseResponseCompression();
            // 允许跨域
            app.UseCors();
            //强制https
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<hubs.gpsHub>("/gpsHub");
            });
        }
    }
}
