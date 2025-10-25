using NLog;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinySocketServer.TCP;

namespace Gateway.jt808
{
    class JT808Session : TcpSession<JT808Server, JT808Session>
    {
        private JT808Protocol protocol = null;
        static private Logger _logger = LogManager.GetCurrentClassLogger();

        private System.Net.Http.HttpClient http = new System.Net.Http.HttpClient();
        public JT808Session()
        {
            protocol = new JT808Protocol();
            protocol.OnDataPacketArrivals += Protocol_OnDataPacketArrivals;
        }

        private void Protocol_OnDataPacketArrivals(byte[] data)
        {
            var cmd = JT808Protocol.GetMessageIDByBody(data);
            var sn = JT808Protocol.GetSerialNumber(data);
            var devId = JT808Protocol.GetDeviceId(data);

            _logger.Debug("JT808接收数据：{0}\t{1}", devId, BitConverter.ToString(data));
            if (cmd == 0x0102)
            {
                var code = JT808Protocol.GetContextBody(data);
                var strCode = Encoding.UTF8.GetString(code);
                _logger.Info("终端请求鉴权:{0} {1} {2}", devId, BitConverter.ToString(code), strCode);
            }
            //应答
            var answer = JT808Protocol.AnswerTerminal(sn, sn, cmd, devId, 0);
            this.Send(answer, 0, answer.Length);

            TransferToServer(data);
        }

        protected override void On_Close()
        {
            if (protocol != null)
            {
                protocol.Close();
                protocol = null;
            }
        }


        protected override void Receive(byte[] data, int offset, int count)
        {
            protocol.Append(data, offset, count);
            protocol.SplitStream();
            base.Receive(data, offset, count);
        }


    }
}
