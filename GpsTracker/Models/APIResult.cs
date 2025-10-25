using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.Models
{
    public class APIResult<T>
    {
        public bool State { get; set; }
        public string Msg { get; set; }
        public T Data { get; set; }
    }
}
