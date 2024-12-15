using Newtonsoft.Json;

namespace RestAPI_Base.Models.Login
{
    public class UserInfoModel
    {
        /// <summary>
        /// 사용자 계정
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 사용자 패스워드
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 이메일
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 사용자 이름
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 연락처
        /// </summary>
        public string contactTelNo { get; set; }
        /// <summary>
        /// 생성일
        /// </summary>
        [JsonIgnore]
        public DateTime joinDate { get; set; }
        /// <summary>
        /// 사용기한
        /// </summary>
        [JsonIgnore]
        public DateTime expireDate { get; set; }
    }
}
