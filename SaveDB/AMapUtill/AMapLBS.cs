using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SaveDB.AMapUtill
{
    static public class AMapLBS
    {


        static public AMapResult GetLBSLocation(string mainlbs, string lbslist)
        {
            const string AMapURL = "http://apilocate.amap.com/position?accesstype=0&cdma=0&bts={0}&nearbts={1}&output=json&key=2cea3de3cb2b6145ba9ade188dbc9e46";
            var url = string.Format(AMapURL, mainlbs, lbslist);

            using var http = new WebClient();
            http.Encoding = UTF8Encoding.UTF8;
            http.Proxy = null;

            var json = http.DownloadString(url);

            var result = JsonSerializer.Deserialize<AMapResult>(json);

            return result;

        }
    }

    #region 数据定义
    public class AMapResult
    {
        public string infocode { get; set; }
        public Location result { get; set; }
        public string info { get; set; }
        public string status { get; set; }
    }

    public class Location
    {
        public string city { get; set; }
        public string province { get; set; }
        public string poi { get; set; }
        public string adcode { get; set; }
        public string street { get; set; }
        public string desc { get; set; }
        public string country { get; set; }
        public string type { get; set; }
        public string location { get; set; }
        public string road { get; set; }
        public string radius { get; set; }
        public string citycode { get; set; }
    }

    #endregion
}
