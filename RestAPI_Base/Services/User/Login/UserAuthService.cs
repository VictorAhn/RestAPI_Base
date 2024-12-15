using Lib.Extension;
using MySQLManager;
using RestAPI_Base.Models.Login;

namespace RestAPI_Base.Services.User.Login
{
    public interface IUserAuthService
    {
        Task<string> GetUserRole(string userID);
        Task<UserLoginResultModel> IsValidUserCredentials(string userID, string password);
    }
    public class UserAuthService : IUserAuthService
    {
        private readonly ILogger<UserAuthService> _logger;
        public UserAuthService(ILogger<UserAuthService> logger)
        {
            _logger = logger;
        }
        public async Task<string> GetUserRole(string userID)
        {
            return "BasicUser";
        }
        public async Task<UserLoginResultModel> IsValidUserCredentials(string userId, string password)
        {
            UserLoginResultModel result = new UserLoginResultModel();
            try
            {
                string xmlPath = Path.Combine("User", "Account", "UserAuth.xml");

                return await MySqlDapperManager.GetQueryFirstFromXmlAsync<UserLoginResultModel>
                    (ConnectionStringEnum.Default, xmlPath, "UserLoginSuccessReturn", new
                    {
                        userId = userId,
                        password = password.ComputeSHA256()
                    });
            }
            catch (Exception exception)
            {
                _logger.LogInformation(exception.Message);
                _logger.LogInformation(exception.StackTrace);
            }

            return null;
        }
    }
}
