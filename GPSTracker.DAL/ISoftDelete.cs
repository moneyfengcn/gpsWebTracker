using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTracker.DAL
{
    public interface ISoftDelete
    {
        /// <summary>
        /// 删除标志
        /// </summary>
        int IsDeleted { get; set; }
        /// <summary>
        /// 创建的时间
        /// </summary>
        DateTime CreateDate { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        DateTime LastUpdate { get; set; }
    }
}
