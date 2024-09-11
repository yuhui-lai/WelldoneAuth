using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelldoneAuth.Lib.Models.Auth
{
    public class PasswordLoginReq
    {
        /// <summary>
        /// 帳號
        /// </summary>
        [Required]
        public string Username { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
