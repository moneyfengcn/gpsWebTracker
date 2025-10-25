using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using Gps.DAL.Entitys;

namespace Gps.DAL
{
    public interface IDbContext
    {
        SqlSugarClient Db { get; } //用来处理事务多表查询和复杂的操作
        SimpleClient<UserProfile> UserProfile { get; }

        SimpleClient<GpsDevices> GpsDevices { get; }
        SimpleClient<HistoryPosition> HistoryPosition { get; }
        SimpleClient<LastPosition> LastPosition { get; }
        SimpleClient<V_LastPosition> V_LastPosition { get; }
        SimpleClient<V_GpsDevice> V_GpsDevice { get; }



    }
}