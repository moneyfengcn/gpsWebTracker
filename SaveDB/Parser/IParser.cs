using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveDB.Parser
{
    public interface IParser
    {
        void Parse(byte[] pack);
    }
}
