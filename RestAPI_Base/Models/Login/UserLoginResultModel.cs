namespace RestAPI_Base.Models.Login
{
    public class UserLoginResultModel
    {
        public string userName { get; set; }
        public string contactTelNo { get; set; }
        public string email { get; set; }
        public string expireDate { get; set; }
        public bool isExpire { get; set; }
    }
}
