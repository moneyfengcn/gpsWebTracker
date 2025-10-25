using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTracker.DAL
{
    public class GpsTrackerContext : IdentityDbContext<GpsTrackerUser, GpsTrackerRole, string>
    {
        private readonly ILogger<GpsTrackerContext> _logger;

        public GpsTrackerContext(DbContextOptions<GpsTrackerContext> options, ILogger<GpsTrackerContext> logger) : base(options)
        {
            _logger = logger;
            ChangeTracker.StateChanged += ChangeTracker_StateChanged;
            ChangeTracker.Tracked += ChangeTracker_StateChanged;
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //生成种子数据
            builder.Seed();

            //反射实体表，给实现了ISoftDelete的加上过滤器
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // 省略其它无关的代码
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    entityType.AddSoftDeleteQueryFilter();
                    _logger.LogInformation("实体表实现软删除接口:{0}  添加过滤器!", entityType.Name);
                }
                else
                {
                    _logger.LogInformation("实体表没有实现软删除接口:{0}", entityType.Name);
                }
            }
        }

        private void ChangeTracker_StateChanged(object? sender, EntityEntryEventArgs e)
        {
            if (e.Entry.Entity is ISoftDelete entityWithTimestamps)
            {
                var now = DateTime.Now;
                switch (e.Entry.State)
                {
                    case EntityState.Deleted:
                        entityWithTimestamps.IsDeleted = 1;
                        entityWithTimestamps.LastUpdate = now;
                        _logger.LogInformation($"软删除拦截 for delete: {e.Entry.Entity}");
                        e.Entry.State = EntityState.Modified;
                        break;
                    case EntityState.Added:
                        entityWithTimestamps.CreateDate = now;
                        entityWithTimestamps.LastUpdate = now;
                        break;
                    case EntityState.Modified:
                        entityWithTimestamps.LastUpdate = now;
                        break;
                }
            }
        }


        public DbSet<Entitys.GpsDevices> GpsDevices { get; set; }
        public DbSet<Entitys.HistoryPosition> HistoryPositions { get; set; }
        public DbSet<Entitys.LastPosition> LastPositions { get; set; }
        public DbSet<Entitys.UserProfile> UserProfiles { get; set; }
    }
}
