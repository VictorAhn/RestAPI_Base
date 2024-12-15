using System.Text.Json.Serialization;

namespace RestAPI_Base.Models.Login
{
    public class LoginResult
    {
        /// <summary>
        /// 사용자 ID
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserID { get; set; }
        /// <summary>
        /// 사용자명
        /// </summary>

        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        /// <summary>
        /// 사용자 연락처
        /// </summary>

        [JsonPropertyName("userTelNo")]
        public string UserTelNo { get; set; }
        /// <summary>
        /// 사용자 이메일
        /// </summary>

        [JsonPropertyName("userEmail")]
        public string UserEmail { get; set; }
        /// <summary>
        /// Access Token
        /// </summary>

        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
        /// <summary>
        /// Refresh Token
        /// </summary>

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
        /// <summary>
        /// 만료일
        /// </summary>
        [JsonPropertyName("expireDate")]
        public string ExpireDate { get; set; }
        /// <summary>
        /// 만료여부
        /// </summary>
        [JsonPropertyName("isExpire")]
        public bool IsExpire { get; set; }
    }
}
