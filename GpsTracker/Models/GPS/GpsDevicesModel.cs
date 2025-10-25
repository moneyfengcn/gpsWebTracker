using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.Models.GPS
{
    public class GpsDevicesModel
    {
        [Required]
        [StringLength(30, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 4)]
        public string DeviceId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string OwnUser { get; set; }
        [Required]
        [StringLength(10, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 1)]
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string DeviceName { get; set; }
        [Required]
        [StringLength(16, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 1)]
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string DeviceType { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Remarks { get; set; }
    }
}
