using System.ComponentModel.DataAnnotations;

namespace WelldoneAuth.Lib.Models.Auth
{
    public class QrcodeLoginReq
    {
        /// <summary>
        /// 帳號
        /// </summary>
        [Required]
        public string Username { get; set; }
        /// <summary>
        /// Device Token
        /// </summary>
        [Required]
        public string DeviceToken { get; set; }
    }
}
