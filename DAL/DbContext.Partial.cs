using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using Gps.DAL.Entitys;

namespace Gps.DAL
{
    partial class DbContext
    {


        #region 私有变量定义
        private SimpleClient<UserProfile> _UserProfile = null;

        private SimpleClient<GpsDevices> _GpsDevices = null;
        private SimpleClient<HistoryPosition> _HistoryPosition = null;
        private SimpleClient<LastPosition> _LastPosition = null;
        private SimpleClient<V_LastPosition> _V_LastPosition = null;
        private SimpleClient<V_GpsDevice> _V_GpsDevice = null;



        #endregion

        //私有变量初始化
        private void On_Initialize()
        {
            #region 私有变量初始化
            _UserProfile = new SimpleClient<UserProfile>(_Db);

            _GpsDevices = new SimpleClient<GpsDevices>(_Db);
            _HistoryPosition = new SimpleClient<HistoryPosition>(_Db);
            _LastPosition = new SimpleClient<LastPosition>(_Db);
            _V_LastPosition = new SimpleClient<V_LastPosition>(_Db);
            _V_GpsDevice = new SimpleClient<V_GpsDevice>(_Db);



            #endregion
        }

        #region 接口实现
        public SimpleClient<UserProfile> UserProfile { get { return _UserProfile; } }

        public SimpleClient<GpsDevices> GpsDevices { get { return _GpsDevices; } }
        public SimpleClient<HistoryPosition> HistoryPosition { get { return _HistoryPosition; } }
        public SimpleClient<LastPosition> LastPosition { get { return _LastPosition; } }
        public SimpleClient<V_LastPosition> V_LastPosition { get { return _V_LastPosition; } }
        public SimpleClient<V_GpsDevice> V_GpsDevice { get { return _V_GpsDevice; } }




        #endregion
    }
}