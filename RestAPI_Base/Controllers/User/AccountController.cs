using Microsoft.AspNetCore.Mvc;
using RestAPI_Base.Models;
using RestAPI_Base.Models.Login;
using RestAPI_Base.Services.User.Login;

namespace RestAPI_Base.Controllers.User
{
    [ApiExplorerSettings(GroupName = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;
        public AccountController(ILogger<AccountController> logger, IAccountService accountService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _accountService = accountService;
            _logger.LogInformation($"{httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString()}: {httpContextAccessor.HttpContext.Request.Path}");
        }
        /// <summary>
        /// 기존 등록된 계정인지 체크
        /// </summary>
        /// <param name="userId">유저 ID</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResponseModel<object>), 200)]
        [HttpPost("CheckAccount")]
        public async Task<ActionResult> UserCreate([FromBody] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return Ok(new ResponseModel<object>(401, "Fail", "요청 데이터 이상", null));

            bool isExist = await _accountService.IsAnExistingUser(userId);
            if (isExist) { return Ok(new ResponseModel<object>(401, "Fail", "이미 등록된 계정입니다.", null)); }
            else return Ok(new ResponseModel<object>(200, "Success", "사용 가능한 계정입니다.", null));
        }
        /// <summary>
        /// 회원가입
        /// </summary>
        /// <param name="userInfo">사용자 입력 정보</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResponseModel<object>), 200)]
        [HttpPost("UserCreate")]
        public async Task<ActionResult> UserCreate([FromBody] UserInfoModel userInfo)
        {
            if (userInfo == null) return Ok(new ResponseModel<object>(401, "Fail", "요청 데이터 이상", null));
            bool isExist = await _accountService.IsAnExistingUser(userInfo.userId);
            if (isExist) { return Ok(new ResponseModel<object>(401, "Fail", "이미 등록된 계정입니다.", null)); }

            bool isCreate = await _accountService.UserCreate(userInfo);
            if (isCreate == false) { return Ok(new ResponseModel<object>(501, "Fail", "작업중 문제가 발생했습니다.", null)); }

            return Ok(new ResponseModel<object>(200, "Success", "회원가입이 완료되었습니다.", null));
        }
    }
}
