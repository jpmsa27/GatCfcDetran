using GatCfcDetran.Services.Dtos.Progress;
using GatCfcDetran.Services.Dtos.User;
using GatCfcDetran.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatCfcDetran.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProgressController(IProgressService progressService) : ControllerBase
    {
        private readonly IProgressService _progressService = progressService;

        [HttpGet("{userCpf}")]
        [Authorize(Policy = "UserAuthenticated")]
        [ProducesResponseType(typeof(ProgressResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProgress(string userCpf)
        {
            var cfcId = User.Claims.FirstOrDefault(x => x.Type == "cfcId")!.Value;
            ProgressResponseDto progress = await _progressService.GetProgress(userCpf, cfcId);
            return Ok(progress);
        }
    }
}
