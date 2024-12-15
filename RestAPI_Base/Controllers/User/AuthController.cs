using Lib.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_Base.Infrastructure;
using RestAPI_Base.Models.Login;
using RestAPI_Base.Models;
using RestAPI_Base.Services.User.Login;
using System.Net;
using System.Security.Claims;

namespace RestAPI_Base.Controllers.User
{
    [ApiExplorerSettings(GroupName = "User")]
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAccountService _accountService;
        private readonly IUserAuthService _userAuthService;
        private readonly IJwtAuthManager _jwtAuthManager;

        public AuthController(ILogger<AuthController> logger, IAccountService accountService, IUserAuthService userAuthService, IJwtAuthManager jwtAuthManager, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _accountService = accountService;
            _userAuthService = userAuthService;
            _jwtAuthManager = jwtAuthManager;
            _logger.LogInformation($"{httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString()}: {httpContextAccessor.HttpContext.Request.Path}");

            var headers = httpContextAccessor.HttpContext.Request.Headers;
            foreach (var header in headers)
            {
                _logger.LogInformation($"{header.Key} : {header.Value}");
            }
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
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            UserLoginResultModel login = await _userAuthService.IsValidUserCredentials(request.userId, request.password);

            if (login == null) return Ok(new ResponseModel<object>(403, "Fail", "계정 정보가 올바르지 않습니다.", null));


            string grade = await _userAuthService.GetUserRole(request.userId);
            if (string.IsNullOrWhiteSpace(grade)) return Ok(new ResponseModel<object>(503, "Fail", "등급정보가 없는 회원입니다.", null));
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,request.userId),
                new Claim(ClaimTypes.Role, AES128DescryptEx.Result(AES128Type.Encrypt, grade)),
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
