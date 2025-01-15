using Lib.Extension;
using MySQLManager;
using RestAPI_Base.Models.Login;
using System.Security.Cryptography;
using System.Text.Json;

namespace RestAPI_Base.Services.User.Login
{
    public interface IAccountService
    {
        Task<bool> IsAnExistingUser(string userId);
        Task<bool> UserCreate(UserInfoModel userInfo);
    }
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        public AccountService(ILogger<AccountService> logger)
        {
            _logger = logger;
        }
        public async Task<bool> IsAnExistingUser(string userId)
        {
            bool result = false;
            try
            {
                string xmlPath = Path.Combine("User", "Account", "UserCreate.xml");

                var query = await MySqlDapperManager.GetQueryFirstFromXmlAsync<int>
                    (ConnectionStringEnum.Default, xmlPath, "CheckUser",
                    new
                    {
                        userId,
                    });

                if (query != 0) return true;
            }
            catch (Exception exception)
            {
                _logger.LogInformation(exception.Message);
                _logger.LogInformation(exception.StackTrace);
            }
            return result;
        }
        public async Task<bool> UserCreate(UserInfoModel userInfo)
        {
            bool result = false;

            MySqlDapperManager mySqlDapperHelper = new MySqlDapperManager(ConnectionStringEnum.Default);

            try
            {
                string xmlPath = Path.Combine("User", "Account", "UserCreate.xml");

                mySqlDapperHelper.BeginTransaction();

                var insertUserInfo = await mySqlDapperHelper.GetQueryFromXmlAsync<bool>(xmlPath, "CreateUser", new
                {
                    userInfo.userId,
                    password = userInfo.password.ComputeSHA256(),
                    userInfo.email,
                    userInfo.userName,
                    userInfo.contactTelNo,
                    usePeriod = userInfo.expireDate
                });

                mySqlDapperHelper.Commit();
            }
            catch (Exception exception)
            {
                mySqlDapperHelper.Rollback();
                _logger.LogInformation(exception.Message);
                _logger.LogInformation(exception.StackTrace);
            }
            return result;
        }
    }
}
