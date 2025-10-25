using NLog;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinySocketServer.UDP;

namespace Gateway.SockerServer.BSJ
{
    public class BSJUdpSesssion : UDPSession<BSJUdpServer, BSJUdpSesssion>
    {
        private BSJProtocol protocol = null;
        static private Logger _logger = LogManager.GetCurrentClassLogger();

        public BSJUdpSesssion()
        {
            protocol = new BSJProtocol(new BSJProtocol.PacketDataArrivalsEx(On_PacketDataArrivalsEx));
        }
        private void On_PacketDataArrivalsEx(byte[] pack)
        {
            var nCommandID = BSJProtocol.GetCommandID(pack);
            var sn = BSJProtocol.GetDeviceID(pack);

            _logger.Debug("BSJ->UDP接收数据：{0}\t\t{1}", sn, BitConverter.ToString(pack));

            //应答
            switch (nCommandID)
            {
                case 0x80:
                case 0x82:
                case 0x84:
                case 0x8D:
                case 0x8E:
                case 0x91:
                case 0x9E:
                case 0xB1:
                case 0xB3:
                case 0xA0:
                case 0xA3:
                case 0x8A:
                case 0xEC:
                case 0xCE:
                case 0xCA:
                case 0xCB:
                    byte[] temp = BSJProtocol.AnswerCommand(pack);
                    this.Send(temp, 0, temp.Length);
                    break;
                default:
                    break;
            }

            TransferToServer(pack);
        }

        protected override void Receive(byte[] data, int offset, int count)
        {
            try
            {
                protocol.Append(data, offset, count);
                protocol.SplitPack();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            base.Receive(data, offset, count);
        }
        protected override void On_Close()
        {
            
        }
    }
}
