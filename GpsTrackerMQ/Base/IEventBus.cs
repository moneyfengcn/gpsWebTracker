using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsTrackerMQ.Base
{
    public interface IEventBus<T>
    {


        void Register(Func<T, bool> callback);
    }
}
