using GpsTrackerMQ;
using Microsoft.Extensions.Logging;
using Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveDB.Parser
{
    public class JT808Parser : IJT808Parser
    {
        private readonly ILogger<JT808Parser> _logger;
        private readonly OutputLocationMQ _output;

        public JT808Parser(ILogger<JT808Parser> logger, OutputLocationMQ output)
        {
            _logger = logger;
            _output = output;
        }

        public void Parse(byte[] pack)
        {
            Task.Run(() =>
            {
                try
                {
                    var cmd = JT808Protocol.GetMessageIDByBody(pack);

                    switch (cmd)
                    {
                        case 0x0200:    //定时回传
                            var location = ParsePositionBlock(pack);
                            if (location != null)
                            {
                                location.Pakcet = BitConverter.ToString(pack);
                                _output.SendGpsLocation(location);
                            }
                            break;
                        case 0x0704:    //补传
                            var locations = ParseBatchPositionBlock(pack);
                            if (locations?.Count > 0)
                            {
                                foreach (var item in locations)
                                {
                                    item.Pakcet = BitConverter.ToString(pack);
                                    _output.SendGpsLocation(item);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            });
        }

        private List<GPS_Location_Model> ParseBatchPositionBlock(byte[] pack)
        {
            var deviceId = JT808Protocol.GetDeviceId(pack);
            var data = JT808Protocol.GetContextBody(pack);

            using var ms = new MemoryStream(data);

            int nItems = ms.ReadByte() << 8;
            nItems += ms.ReadByte();
            int type = ms.ReadByte();

            Console.WriteLine("0704回传类型：{0}", (type == 0 ? "正常上传" : "盲区补传"));

            List<GPS_Location_Model> result = new List<GPS_Location_Model>();

            for (int i = 0; i < nItems; i++)
            {
                //子项长度
                int nSublen = ms.ReadByte() << 8;
                nSublen += ms.ReadByte();

                byte[] item = new byte[nSublen];
                ms.Read(item, 0, item.Length);
                var location = GetLocation(item, deviceId);

                result.Add(location);
            }

            return result;
        }

        #region 解析数据包

        private GPS_Location_Model GetLocation(byte[] data, string deviceId)
        {
            uint alarm = GetDWord(data, 0);
            uint status = GetDWord(data, 4);
            double Latitude = GetDWord(data, 8);
            double Longitude = GetDWord(data, 12);
            ushort h = GetWord(data, 16);
            ushort v = GetWord(data, 18);
            ushort angle = GetWord(data, 20);

            DateTime dtTime;

            string strTime = "20" + data[22].ToString("X2") + "-" +
                                   data[23].ToString("X2") + "-" +
                                   data[24].ToString("X2") + " " +
                                   data[25].ToString("X2") + ":" +
                                   data[26].ToString("X2") + ":" +
                                   data[27].ToString("X2");


            if (!DateTime.TryParse(strTime, out dtTime))
            {
                dtTime = DateTime.Now;               
            }

            //纬度
            double lat = (double)Latitude / 1000000d;
           
            //经度
            var lng = (double)Longitude / 1000000d;
  
            //速度
            var speed = (double)v / 10d;
 
            //方向
            //string strAngle = angle.ToString();

            var location = new GPS_Location_Model()
            {
                Angle = angle,
                DeviceId = deviceId,
                Lat = lat,
                Lng = lng,
                Speed = (float)speed,
                Time = dtTime
            };

            return location;
        }
        /// <summary>
        /// 解析定位数据块
        /// </summary>
        /// <param name="data">数据包</param>
        /// <param name="offset">定位数据起始位置</param>
        /// <param name="vehicle"></param>
        private GPS_Location_Model ParsePositionBlock(byte[] data)
        {
            var deviceId = JT808Protocol.GetDeviceId(data);
            uint alarm = GetDWord(data, 13);
            uint status = GetDWord(data, 17);
            double Latitude = GetDWord(data, 21);
            double Longitude = GetDWord(data, 25);
            ushort h = GetWord(data, 29);
            ushort v = GetWord(data, 31);
            ushort angle = GetWord(data, 33);
            DateTime dtTime;

            string strTime = "20" + data[35].ToString("X2") + "-" +
                                   data[36].ToString("X2") + "-" +
                                   data[37].ToString("X2") + " " +
                                   data[38].ToString("X2") + ":" +
                                   data[39].ToString("X2") + ":" +
                                   data[40].ToString("X2");


            if (!DateTime.TryParse(strTime, out dtTime))
            {
                dtTime = DateTime.Now;
                strTime = dtTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            //纬度
            double lat = (double)Latitude / 1000000d;
            string strLatitude = lat.ToString("0.00000");


            //经度
            var lng = (double)Longitude / 1000000d;
            string strLongitude = lng.ToString("0.00000");

            //速度
            var speed = (double)v / 10d;
            string strSpeed = speed.ToString("0.0");


            //方向
            string strAngle = angle.ToString();




            var location = new GPS_Location_Model()
            {
                Angle = angle,
                DeviceId = deviceId,
                Lat = lat,
                Lng = lng,
                Speed = (float)speed,
                Time = dtTime
            };

            return location;
        }

        private ushort GetWord(byte[] data, int Index)
        {
            byte[] temp = new byte[2] { data[Index + 1], data[Index] };
            return BitConverter.ToUInt16(temp, 0);
        }
        private uint GetDWord(byte[] data, int Index)
        {
            byte[] temp = new byte[4] { data[Index + 3], data[Index + 2], data[Index + 1], data[Index] };
            return BitConverter.ToUInt32(temp, 0);
        }
        #endregion


    }

    public interface IJT808Parser : IParser { }
}
