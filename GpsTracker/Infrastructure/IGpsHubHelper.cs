using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.hubs
{
    public interface IGpsHubHelper : IGpsEvents
    {

        void OnConnectedAsync(string userName, string connectionId);


        void OnDisconnectedAsync(string connectionId);

    }
}
