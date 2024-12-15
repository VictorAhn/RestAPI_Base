using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace RestAPI_Base.Models.Login
{
    public class LoginRequest
    {
        /// <summary>
        /// 사용자 ID
        /// </summary>
        [Required]
        [DefaultValue("test_id")]
        public string userId { get; set; }
        /// <summary>
        /// 사용자 PW
        /// </summary>
        [Required]
        [DefaultValue("test_pw")]
        public string password { get; set; }
    }
}
