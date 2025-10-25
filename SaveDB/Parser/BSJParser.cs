using GpsTrackerMQ;
using Microsoft.Extensions.Logging;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveDB.Parser
{
    public class BSJParser : IBSJParser
    {
        private readonly ILogger<BSJParser> _logger;

        private readonly OutputLocationMQ _output;

        public BSJParser(ILogger<BSJParser> logger, OutputLocationMQ output)
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
                    var cmd = BSJProtocol.GetCommandID(pack);

                    switch (cmd)
                    {
                        case 0x80:
                        case 0x81:
                            var location = Parse80Position(pack);
                            if (location != null)
                            {
                                location.Pakcet = BitConverter.ToString(pack);
                                _output.SendGpsLocation(location);
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

        private GPS_Location_Model Parse80Position(byte[] pack)
        {
            if (pack.Length < 40) return null;
            var deviceId = BSJProtocol.GetDeviceID(pack);
            string strTemp;
            byte[] buff;

            if (!BSJProtocol.TryParse(pack, out buff)) return null;
            string strTime = "20" + buff[0].ToString("X2") + "-" +
                                  buff[1].ToString("X2") + "-" +
                                  buff[2].ToString("X2") + " " +
                                  buff[3].ToString("X2") + ":" +
                                  buff[4].ToString("X2") + ":" +
                                  buff[5].ToString("X2");
            DateTime dtTime;
            if (!DateTime.TryParse(strTime, out dtTime))
            {
                dtTime = DateTime.Now;
            }

            //纬度
            strTemp = buff[6].ToString("X2") + buff[7].ToString("X2") + buff[8].ToString("X2") + buff[9].ToString("X2");
            var s1 = strTemp.Substring(0, 3);
            var s2 = strTemp.Substring(3, 2) + "." + strTemp.Substring(5);
            double.TryParse(s1, out double L1);
            double.TryParse(s2, out double L2);

            double lat = L1 + (L2 / 60D);

            //经度
            strTemp = buff[10].ToString("X2") + buff[11].ToString("X2") + buff[12].ToString("X2") + buff[13].ToString("X2");
            s1 = strTemp.Substring(0, 3);
            s2 = strTemp.Substring(3, 2) + "." + strTemp.Substring(5);
            double.TryParse(s1, out L1);
            double.TryParse(s2, out L2);

            double lng = L1 + (L2 / 60D);

            //速度
            strTemp = buff[14].ToString("X2") + buff[15].ToString("X2");
            double.TryParse(strTemp, out double speed);

            //方向       
            strTemp = buff[16].ToString("X2") + buff[17].ToString("X2");
            double.TryParse(strTime, out double angle);

            var status = buff[18];
            var locate = (status & 0x80) != 0;

            //判断是否己定位
            if (locate)
            {
                return new GPS_Location_Model()
                {
                    Angle = (float)angle,
                    DeviceId = deviceId,
                    Lat = lat,
                    Lng = lng,
                    Speed = (float)speed,
                    Time = dtTime,
                };
            }
            else
            {
                //GPS未定位的话，则判断是否有基站数据 
                //通过基站来定位 
                _logger.LogInformation("GPS未定位,DeviceID:{0}", deviceId);
                //判断是否有扩展数据
                var len = pack.Length - (9 + 34 + 2);
                if (len <= 0)
                {
                    //没有
                    return null;
                }
                else
                {
                    using var ms = new System.IO.MemoryStream();
                    ms.Write(pack, 9 + 34, len);
                    ms.Seek(0L, System.IO.SeekOrigin.Begin);
                    while (ms.Position < ms.Length)
                    {
                        var subCMD = ((int)ms.ReadByte() << 8) + (int)ms.ReadByte();
                        var temp = new byte[subCMD];
                        ms.Read(temp, 0, temp.Length);

                        switch (subCMD)
                        {
                            case 0x0024:
                                if (temp[0] == 0x00 && temp[1] == 0xA9)
                                {
                                    var index = 2;
                                    var wMCC_Hi = (int)temp[index++];
                                    var wMCC_Low = (int)temp[index++];
                                    var MNC = (int)temp[index++];

                                    var wMCC = (wMCC_Hi << 8) | wMCC_Low;

                                    var Num = (int)temp[index++];

                                    if (Num <= 0)
                                    {
                                        _logger.LogInformation("设备:{0} GPS不定位，己没有LBS数据", deviceId);
                                    }
                                    else
                                    {

                                        var lbsList = new List<string>();
                                        for (int i = 0; i < Num; i++)
                                        {
                                            var wLac_Hi = (int)temp[index++];
                                            var wLac_Low = (int)temp[index++];
                                            var wLac = (wLac_Hi << 8) | wLac_Low;

                                            var wCellId_Hi = (int)temp[index++];
                                            var wCellId_Low = (int)temp[index++];
                                            var wCellId = (wCellId_Hi << 8) | wCellId_Low;

                                            var RxLev = (int)temp[index++];

                                            lbsList.Add(string.Format("{0},{1},{2}", wLac.ToString(), wCellId.ToString(), RxLev.ToString()));
                                        }

                                        var mainlbs = string.Format("{0},{1},{2}", wMCC, MNC, lbsList[0]);
                                        var lbs = string.Join("|", lbsList);

                                        var lbsLocation = AMapUtill.AMapLBS.GetLBSLocation(mainlbs, lbs);
                                        if (lbsLocation.status == "1")
                                        {
                                            _logger.LogInformation("LBS定位：{0} {1} {2}", deviceId, lbsLocation.result.location, lbsLocation.result.desc);

                                            var lng_lat = lbsLocation.result.location.Split(',');
                                            lng = double.Parse(lng_lat[0]);
                                            lat = double.Parse(lng_lat[1]);
                                            return new GPS_Location_Model()
                                            {
                                                Angle = 0,
                                                DeviceId = deviceId,
                                                Lat = lat,
                                                Lng = lng,
                                                Speed = 0,
                                                Time = DateTime.Now,
                                            };
                                        }
                                    }
                                }

                                break;
                            default:
                                break;
                        }
                    }


                }
                return null;
            }
        }
    }

    public interface IBSJParser : IParser { }
}
