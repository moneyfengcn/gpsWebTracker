using System;
using System.IO;

namespace Protocol
{
    public class BSJProtocol : IDisposable
    {

        public delegate void PacketDataArrivalsEx(byte[] packData);
        public event PacketDataArrivalsEx OnPacketDataArrivals;

        private MemoryStream m_Buffer = null;


        public BSJProtocol(PacketDataArrivalsEx packetEvent)
        {
            OnPacketDataArrivals += packetEvent;
            m_Buffer = new MemoryStream();
        }



        public void Append(byte[] buff, int offset, int count)
        {
            m_Buffer.Write(buff, offset, count);
        }



        public void Clear()
        {
            m_Buffer.Seek(0, SeekOrigin.Begin);
        }



        public void SplitPack()
        {

            byte[] temp = m_Buffer.GetBuffer();
            int nTotal = (int)m_Buffer.Position;

            int nLen = nTotal - 4;
            int nPos = 0;

            //遍历所有数据

            for (int i = 0; i < nLen; i++)
            {
                //2929头

                if ((temp[i] == 0x29) && (temp[i + 1] == 0x29))
                {
                    int nPackLen = temp[i + 3] << 8;
                    nPackLen += temp[i + 4];

                    //判断这个包是否收齐

                    if ((i + nPackLen + 4) <= nTotal)
                    {
                        //包尾
                        if (temp[i + nPackLen + 4] == 0x0D)
                        {
                            //校验
                            if (temp[i + nPackLen + 3] == GetXorValue(temp, i, nPackLen + 3))
                            {
                                //将包数据截取下来
                                byte[] pack = new byte[nPackLen + 5];
                                Buffer.BlockCopy(temp, i, pack, 0, pack.Length);

                                OnPacketDataArrivals(pack);

                                nPos = i + nPackLen + 5;
                                i = nPos - 1;
                            }
                        }
                    }
                }
            }


            //如果己被截取了数据，则需要将己取的数据除掉

            if (nPos > 0)
            {
                if (nPos < nTotal)
                {
                    byte[] xxoo = new byte[nTotal - nPos];
                    Buffer.BlockCopy(temp, nPos, xxoo, 0, xxoo.Length);
                    Clear();
                    Append(xxoo, 0, xxoo.Length);
                }
                else
                {
                    Clear();
                }
            }

        }

        static public string GetDeviceID(byte[] pack)
        {
            return pack[5].ToString("X2") +
               pack[6].ToString("X2") +
               pack[7].ToString("X2") +
               pack[8].ToString("X2");
        }
        static public int GetCommandID(byte[] pack)
        {
            return pack[2];
        }
        /// <summary>
        /// 计算校验值
        /// </summary>
        /// <param name="abtData">用来计算的数组</param>
        /// <param name="iStartPos">开始计算的索引点</param>
        /// <param name="nLength">计算的长度</param>
        /// <returns></returns>
        public static byte GetXorValue(byte[] abtData, int iStartPos, int nLength)
        {
            byte xorValue = abtData[iStartPos];

            nLength += iStartPos;
            iStartPos += 1;
            for (int i = iStartPos; i < nLength; i++)
            {
                xorValue ^= abtData[i];
            }
            return xorValue;
        }

        /// <summary>
        /// 生成应答指令包
        /// </summary>
        /// <param name="cmdBuff"></param>
        /// <returns></returns>
        public static byte[] AnswerCommand(byte[] cmdBuff)
        {
            byte[] buff = new byte[10] { 0x29, 0x29, 0x21, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x0D };

            buff[5] = cmdBuff[cmdBuff.Length - 2];
            buff[6] = cmdBuff[2];
            if (cmdBuff.Length > 10)
            {
                buff[7] = cmdBuff[9];
            }

            buff[8] = GetXorValue(buff, 0, 8);
            return buff;
        }


        /// <summary>
        /// 拼合2929包
        /// </summary>
        /// <param name="nCmdID">信令号</param>
        /// <param name="deviceID">设备ID</param>
        /// <param name="Content">指令内容</param>
        /// <returns></returns>
        public static byte[] CombinePacket(byte nCmdID, string deviceID, byte[] Content)
        {
            byte[] buff = new byte[Content.Length + 11];
            buff[0] = 0x29;
            buff[1] = 0x29;

            buff[2] = nCmdID;

            buff[3] = (byte)(((buff.Length - 5) & 0x0000ff00) >> 8);
            buff[4] = (byte)((buff.Length - 5) & 0x000000ff);



            buff[5] = (byte)Convert.ToInt32(deviceID.Substring(0, 2), 16);
            buff[6] = (byte)Convert.ToInt32(deviceID.Substring(2, 2), 16);
            buff[7] = (byte)Convert.ToInt32(deviceID.Substring(4, 2), 16);
            buff[8] = (byte)Convert.ToInt32(deviceID.Substring(6, 2), 16);

            Buffer.BlockCopy(Content, 0, buff, 9, Content.Length);

            buff[buff.Length - 2] = GetXorValue(buff, 0, buff.Length - 2);
            buff[buff.Length - 1] = 0x0d;

            return buff;
        }




        /// <summary>
        /// 从2929包中抽取内容
        /// </summary>
        /// <param name="PackData">2929包</param>
        /// <param name="ContextData">抽取出来的内容</param>
        /// <returns>解析成功返回真</returns>
        public static bool TryParse(byte[] PackData, out byte[] ContextData)
        {
            ContextData = null;
            try
            {
                if ((PackData[0] == 0x29) && (PackData[1] == 0x29))
                {
                    if (PackData[PackData.Length - 1] == 0x0d)
                    {
                        if (PackData.Length > 11)
                        {
                            ContextData = new byte[PackData.Length - 11];
                            Buffer.BlockCopy(PackData, 9, ContextData, 0, ContextData.Length);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {

                return false;
            }
        }




        #region IDisposable 成员

        public void Dispose()
        {
            if (m_Buffer != null)
            {
                m_Buffer.Dispose();
            }
        }

        #endregion
    }
}
