using Microsoft.AspNetCore.Mvc;

namespace RestAPI_Base.Controllers
{
    [ApiExplorerSettings(GroupName = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }
    }
}
