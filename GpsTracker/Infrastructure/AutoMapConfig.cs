using GPSTracker.DAL.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.Infrastructure
{
    public class AutoMapConfig : AutoMapper.Profile
    {
        public AutoMapConfig()
        {
            _CreateMap<GpsDevices, Models.GPS.GpsDevicesModel>();

        }
        private void _CreateMap<TSource, TDestination>()
        {
            CreateMap<TSource, TDestination>();
            CreateMap<TDestination, TSource>();
        }
    }
}
