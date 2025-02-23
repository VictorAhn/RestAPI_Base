using Lib.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_Base.Infrastructure;
using RestAPI_Base.Models;
using RestAPI_Base.Models.Login;
using RestAPI_Base.Services.Admin;
using RestAPI_Base.Services.User.Login;
using System.Net;
using System.Security.Claims;

namespace RestAPI_Base.Controllers
{
    [ApiExplorerSettings(GroupName = "Admin")]
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminService _adminService;
        private readonly IJwtAuthManager _jwtAuthManager;

        public AdminController(ILogger<AdminController> logger, IAdminService adminService, IJwtAuthManager jwtAuthManager, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _adminService = adminService;
            _jwtAuthManager = jwtAuthManager;
        }
        /// <summary>
        /// 로그인
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseModel<LoginResult>), 200)]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            IPAddress ip = Request.HttpContext.Connection.RemoteIpAddress;
            if (ip != null)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) ip = Dns.GetHostEntry(ip).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                _logger.LogInformation($"Client IP : [{ip.ToString()}]");
            }

            UserLoginResultModel login = await _adminService.IsValidUserCredentials(request.userId, request.password);

            if (login == null) return Ok(new ResponseModel<object>(403, "Fail", "계정 정보가 올바르지 않습니다.", null));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name,request.userId),
                new Claim(ClaimTypes.Role, AES128DescryptEx.Result(AES128Type.Encrypt, "Admin_User")),
                new Claim(ClaimTypes.SerialNumber, "S_Number"),
                new Claim(ClaimTypes.Webpage, ProjectInfo.HostName)
            };

            var jwtResult = _jwtAuthManager.GenerateTokens(request.userId, claims, DateTime.Now);
            _logger.LogInformation($"User [{request.userId}] logged in the system.");

            return Ok(new ResponseModel<LoginResult>(200, "Success", "", new LoginResult
            {
                UserID = request.userId,
                AccessToken = jwtResult.AccessToken,
                UserName = login.userName,
                UserTelNo = login.contactTelNo,
                UserEmail = login.email,
                RefreshToken = jwtResult.RefreshToken.TokenString,
                ExpireDate = login.expireDate,
                IsExpire = login.isExpire
            }));
        }
    }
}
