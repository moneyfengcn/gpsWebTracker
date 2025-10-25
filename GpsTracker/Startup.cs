
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
            //����ѹ��
            services.AddResponseCompression();
            //�������
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
                //��ʾ�ͻ��������3min��û��������������κ���Ϣ����ô�������������Ϊ�ͻ����Ѿ��Ͽ�������
                option.ClientTimeoutInterval = TimeSpan.FromMinutes(3);
                //��ʾ���������δ��15s����ͻ��˷�����Ϣ����15s��ʱ����������Զ�ping�ͻ��ˣ������ӱ��ִ򿪵�״̬��
                //option.KeepAliveInterval = TimeSpan.FromSeconds(60);
            });
            services.AddAutoMapper(typeof(Program).Assembly);
            //services.AddTransient<IDbContext>(a => new Gps.DAL.DbContext(Configuration.GetConnectionString("GpsTrackerContextConnection"), a.GetService<ILogger<Gps.DAL.DbContext>>()));
            services.AddEntityCore(Configuration.GetConnectionString("GpsTrackerContextConnection"));
            services.AddMyIdentityConfig();

#if !false
            //һ��Ҫ����
            services.AddSingleton<IGpsHubHelper, GpsHubHelper>();
            //��Ϣ����
            services.AddSingleton<IGpsLocationMQ, RecevieGpsLocationMQ>();
            //���շ���
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
            // �������
            app.UseCors();
            //ǿ��https
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
