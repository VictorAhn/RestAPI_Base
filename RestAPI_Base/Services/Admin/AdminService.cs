using Lib.Extension;
using MySQLManager;
using RestAPI_Base.Models.Login;

namespace RestAPI_Base.Services.Admin
{
    public interface IAdminService
    {
        Task<UserLoginResultModel> IsValidUserCredentials(string userId, string password);
    }
    public class AdminService : IAdminService
    {
        private readonly ILogger<AdminService> _logger;
        public AdminService(ILogger<AdminService> logger)
        {
            _logger = logger;
        }
        public async Task<UserLoginResultModel> IsValidUserCredentials(string userId, string password)
        {
            UserLoginResultModel result = new UserLoginResultModel();
            try
            {
                string xmlPath = Path.Combine("Admin", "AdminAuth.xml");

                return await MySqlDapperManager.GetQueryFirstFromXmlAsync<UserLoginResultModel>
                    (ConnectionStringEnum.Default, xmlPath, "AdminLoginSuccessReturn", new
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
