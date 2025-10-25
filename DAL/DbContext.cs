using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Gps.DAL.Entitys;
using Microsoft.Extensions.Logging;

namespace Gps.DAL
{
    public partial class DbContext : IDbContext, IDisposable
    {
        private SqlSugarClient _Db = null;

        public SqlSugarClient Db { get { return _Db; } } //用来处理事务多表查询和复杂的操作


        public DbContext(string connStr, ILogger<DbContext> logger)
        {
            _Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connStr,
                DbType = DbType.SqlServer,
                InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样

            });
            _Db.Open();
            //调式代码 用来打印SQL 
            _Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                logger.LogDebug("执行SQL: {0} \r\n {1}", sql,
                  Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
            };

            On_Initialize();
        }



        #region Dispose实现
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }
                if (_Db != null)
                {
                    _Db.Dispose();
                    _Db = null;

                    //Console.WriteLine("DB对象销毁");
                }
                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~DbContext()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion



    }


    static public class DbContextExtensions
    {
        static public Expressionable<T> CreateWhereExp<T>(this SimpleClient<T> obj) where T : class, new()
        {
            return Expressionable.Create<T>();
        }
    }
}