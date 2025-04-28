using GatCfcDetran.Services.Dtos.Cfc;
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
    public class CfcsController(ICfcServices cfcServices) : ControllerBase
    {
        private readonly ICfcServices _cfcServices = cfcServices;

        [HttpPost]
        [Authorize(Policy = "CfcPolicy")]
        [ProducesResponseType(typeof(CreateCfcResponseDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCfc(CreateCfcRequestDto requestDto)
        {
            var cfcId = User.Claims.FirstOrDefault(x => x.Type == "cfcId")!.Value;
            CreateCfcResponseDto userCreated = await _cfcServices.CreateCfc(requestDto, cfcId);
            return Created($"api/Cfcs/{userCreated.Id}", userCreated);
        }

        [HttpGet("{cnpj}")]
        [Authorize(Policy = "CfcAdminPolicy")]
        [ProducesResponseType(typeof(CreateCfcResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCfc(string cnpj)
        {
            CreateCfcResponseDto userCreated = await _cfcServices.GetCfc(cnpj);
            return Ok(userCreated);
        }
    }
}
