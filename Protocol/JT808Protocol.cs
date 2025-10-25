using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public class JT808Protocol
    {
        private class MemoryBuffer
        {
            private const int MAX_BUFFER_SIZE = 65536;
            private byte[] m_buff = null;
            private int m_DataSize = 0;
            private int m_writePos = 0;

            public MemoryBuffer()
            {
                m_buff = new byte[MAX_BUFFER_SIZE];
                m_DataSize = 0;
                m_writePos = 0;
            }

            public void Write(byte[] data, int offset, int length)
            {
                if (length <= 0) return;
                if ((length + m_writePos) >= m_buff.Length)
                {
                    m_DataSize = 0;
                    m_writePos = 0;
                }
                Buffer.BlockCopy(data, offset, m_buff, m_writePos, length);
                m_DataSize += length;
                m_writePos += length;
            }

            public void Clear()
            {
                m_DataSize = 0;
                m_writePos = 0;
            }


            public void Seek(int nPos)
            {
                if (nPos == 0) return;
                if (nPos >= (m_DataSize))
                {
                    m_DataSize = 0;
                    m_writePos = 0;
                }
                else
                {
                    int nCount = (m_DataSize - nPos);

                    Buffer.BlockCopy(m_buff, nPos, m_buff, 0, nCount);
                    m_DataSize = nCount;
                    m_writePos = nCount;
                }
            }

            public byte[] GetBuffer
            {
                get { return m_buff; }
            }

            public int Count { get { return m_DataSize; } }

            public void Close()
            {
                if (m_buff != null)
                {
                    m_buff = null;
                    m_DataSize = 0;
                    m_writePos = 0;
                }
            }
        }

        public delegate void OnDataPacketArrivalsEx(byte[] data);

        public event OnDataPacketArrivalsEx OnDataPacketArrivals;


        private MemoryBuffer _buff = new MemoryBuffer();
        //private object _objLock = new object();

        public void Append(byte[] data)
        {
            _buff.Write(data, 0, data.Length);
        }

        public void Append(byte[] data, int nSize)
        {
            _buff.Write(data, 0, nSize);
        }

        public void Append(byte[] data, int offset, int nSize)
        {
            _buff.Write(data, offset, nSize);
        }

        public void SplitStream()
        {
            if (_buff == null) return;

            byte[] temp = _buff.GetBuffer;
            int nSize = _buff.Count;

            bool blnFlag = false;
            int nPos = 0;
            for (int i = 0; i < nSize; i++)
            {
                if (temp[i] == 0x7E)
                {
                    if (!blnFlag)
                    {
                        blnFlag = true;
                        nPos = i;
                    }
                    else
                    {
                        if (i - nPos > 10)
                        {
                            byte[] pack = new byte[(i + 1) - nPos];
                            Buffer.BlockCopy(temp, nPos, pack, 0, pack.Length);
                            TransProc(pack);
                            blnFlag = false;
                            nPos = i + 1;
                        }
                        else
                        {
                            nPos = i;
                        }
                    }
                }
            }

            if (nPos > 0)
            {
                if (nPos >= nSize)
                {
                    _buff.Clear();
                }
                else
                {
                    _buff.Seek(nPos);
                }
            }

        }

        private void TransProc(byte[] data)
        {
            byte[] xxoo = UnEscape(data);
            if (OnDataPacketArrivals != null)
            {
                OnDataPacketArrivals(xxoo);
            }
        }

        public void Close()
        {
            if (_buff != null)
            {
                _buff.Close();
                _buff = null;
            }
        }


        static byte[] _x7E = new byte[] { 0x7D, 0x02 };
        static byte[] _x7D = new byte[] { 0x7D, 0x01 };

        /// <summary>
        /// 转义
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public byte[] Escape(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteByte(data[0]);
                int nCount = data.Length - 1;
                for (int i = 1; i < nCount; i++)
                {
                    int Temp = data[i];
                    switch (Temp)
                    {
                        case 0x7E:
                            ms.Write(_x7E, 0, 2);
                            break;
                        case 0x7D:
                            ms.Write(_x7D, 0, 2);
                            break;
                        default:
                            ms.WriteByte(data[i]);
                            break;
                    }
                }

                ms.WriteByte(data[0]);

                byte[] xxoo = new byte[ms.Position];
                ms.Seek(0L, SeekOrigin.Begin);
                ms.Read(xxoo, 0, xxoo.Length);
                return xxoo;
            }
        }

        static public byte[] UnEscape(byte[] data)
        {
            byte xxxxx = 0x7e;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteByte(data[0]);

                int nCount = data.Length - 1;
                int i = 1;
                while (i < nCount)
                {
                    if (data[i] == 0x7D)
                    {
                        int Temp = data[i + 1];
                        switch (Temp)
                        {
                            case 0x01:
                                ms.WriteByte(data[i]);
                                i += 2;
                                break;
                            case 0x02:
                                ms.WriteByte(xxxxx);
                                i += 2;
                                break;
                            default:
                                ms.WriteByte(data[i]);
                                i++;
                                break;
                        }
                    }
                    else
                    {
                        ms.WriteByte(data[i]);
                        i++;
                    }
                }

                ms.WriteByte(data[0]);

                byte[] xxoo = new byte[ms.Position];
                ms.Seek(0L, SeekOrigin.Begin);
                ms.Read(xxoo, 0, xxoo.Length);
                return xxoo;
            }
        }

        /// <summary>
        /// 判断数据包中的手机位是否是合格的手机号码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        //public static bool IsMobile(byte[] data)
        //{
        //    const string MOBILE_STRING = "0123456789";
        //    string xx = data[5].ToString("X2") +
        //                data[6].ToString("X2") +
        //                data[7].ToString("X2") +
        //                data[8].ToString("X2") +
        //                data[9].ToString("X2") +
        //                data[10].ToString("X2");

        //    string temp = xx.Substring(0, 3);
        //    switch (temp)
        //    {
        //        case "013":
        //        case "014":
        //        case "015":
        //        case "018":
        //            for (int i = 0; i < xx.Length; i++)
        //            {
        //                if (MOBILE_STRING.IndexOf(xx[i]) < 0)
        //                {
        //                    return false;
        //                }
        //            }
        //            return true;
        //        default:
        //            return false;
        //    }

        //}

        //从数据包中抽出载荷
        public static byte[] GetContextBody(byte[] pack)
        {
            int len = GetPacketDataLength(pack);
            byte[] body = new byte[len];
            Buffer.BlockCopy(pack, 13, body, 0, body.Length);

            return body;
        }
        //获取包中的数据长度
        static public int GetPacketDataLength(byte[] pack)
        {
            int len = ((int)pack[3] << 8 | (int)pack[4]);
            return len;
        }
        public static string GetDeviceId(byte[] data)
        {
            string strText = data[5].ToString("X2");

            for (int i = 0; i < 5; i++)
            {
                strText += data[6 + i].ToString("X2");
            }

            return strText;
        }

        public static ushort GetMessageIDByBody(byte[] data)
        {
            byte[] temp = new byte[2] { data[2], data[1] };
            return BitConverter.ToUInt16(temp, 0);
        }

        public static ushort GetSerialNumber(byte[] data)
        {
            byte[] temp = new byte[2] { data[12], data[11] };
            return BitConverter.ToUInt16(temp, 0);
        }

        public static byte[] MakeCommand(ushort MessageID,
                                        string Mobile,
                                        ushort SN,
                                        byte[] Content)
        {
            byte[] temp;
            using var ms = new MemoryStream();

            temp = BitConverter.GetBytes(MessageID);

            ms.WriteByte(temp[1]);
            ms.WriteByte(temp[0]);

            ms.Write(new byte[] { 0x00, 0x00 }, 0, 2);

            Mobile = Mobile.PadLeft(12, '0');
            int nPos = 0;
            for (int i = 0; i < 6; i++)
            {
                byte b = Convert.ToByte(Mobile.Substring(nPos, 2), 16);
                ms.WriteByte(b);
                nPos += 2;
            }

            temp = BitConverter.GetBytes(SN);
            ms.WriteByte(temp[1]);
            ms.WriteByte(temp[0]);

            if (Content != null)
            {
                ms.Write(Content, 0, Content.Length);
            }

            ms.WriteByte(0x00);

            byte[] buff = new byte[(int)ms.Position];

            ms.Seek(0L, SeekOrigin.Begin);
            ms.Read(buff, 0, buff.Length);

            if (Content != null)
            {
                temp = BitConverter.GetBytes((ushort)Content.Length);
                buff[2] = temp[1];
                buff[3] = temp[0];
            }

            buff[buff.Length - 1] = GetXorValue(buff, 0, buff.Length - 1);

            byte[] xxoo = new byte[buff.Length + 2];
            xxoo[0] = 0x7E;
            xxoo[xxoo.Length - 1] = 0x7E;
            Buffer.BlockCopy(buff, 0, xxoo, 1, buff.Length);
            buff = Escape(xxoo);

            return buff;

        }



        public static byte GetXorValue(byte[] data, int StartIndex, int EndPosition)
        {
            byte temp = data[StartIndex];
            for (int i = (StartIndex + 1); i <= EndPosition; i++)
            {
                temp ^= data[i];
            }
            return temp;
        }




        public static byte[] AnswerTerminal(int sn, int answerSN, int MessageID, string strMobile, byte Paramter)
        {
            byte[] temp;

            switch (MessageID)
            {
                case 0x0100:
                    temp = new byte[13]{(byte)(answerSN>>8),(byte)(answerSN & 0xff),Paramter,
                                          0x66 ,0x65 ,0x6e ,0x78 ,0x69 ,0x61 ,0x6e ,0x67,0x75,0x6f};
                    return MakeCommand(0x8100, strMobile, (ushort)sn, temp);
                case 0x0102:
                    temp = new byte[5];
                    temp[0] = (byte)((answerSN >> 8) & 0xff);
                    temp[1] = (byte)(answerSN & 0xFF);

                    temp[2] = (byte)((MessageID >> 8) & 0xff);
                    temp[3] = (byte)(MessageID & 0xFF);
                    temp[4] = Paramter;
                    return MakeCommand(0x8001, strMobile, (ushort)sn, temp);
                default:
                    temp = new byte[5];
                    temp[0] = (byte)((answerSN >> 8) & 0xff);
                    temp[1] = (byte)(answerSN & 0xFF);

                    temp[2] = (byte)((MessageID >> 8) & 0xff);
                    temp[3] = (byte)(MessageID & 0xFF);
                    temp[4] = 0;

                    return MakeCommand(0x8001, strMobile, (ushort)sn, temp);
            }
        }
    }
}
