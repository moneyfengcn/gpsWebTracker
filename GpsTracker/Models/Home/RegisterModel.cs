using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.Models.Home
{
    public class RegisterModel
    {

        [Required]//必须数据
        [StringLength(10)]//最大长度10
        public string UserName { get; set; }
        [Required]//必须数据
        [StringLength(16)]//最大长度10
        public string Password { get; set; }
        [Required]//必须数据
        [StringLength(16)]//最大长度10
        public string ConfirmPassword { get; set; }

        [Required]//必须数据
        [StringLength(30)]//最大长度30
        public string Email { get; set; }
        /// <summary>
        /// 注册验证码
        /// </summary>
        public string Code { get; set; }
    }
}
