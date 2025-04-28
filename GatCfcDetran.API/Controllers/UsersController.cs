using GatCfcDetran.Services.Dtos;
using GatCfcDetran.Services.Dtos.User;
using GatCfcDetran.Services.Interface;
using GatCfcDetran.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatCfcDetran.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost]
        [Authorize(Policy = "CfcPolicy")]
        [ProducesResponseType(typeof(CreateUserResponseDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUser(CreateUserRequestDto requestDto)
        {
            var cfcId = User.Claims.FirstOrDefault(x => x.Type == "cfcId")!.Value;
            CreateUserResponseDto userCreated = await _userService.CreateUser(requestDto, cfcId);
            return Created($"api/Users/{userCreated.Id}",userCreated);
        }

        [Authorize(Policy = "UserAuthenticated")]
        [HttpGet("{cpf}")]
        [ProducesResponseType(typeof(CreateUserResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUser(string cpf)
        {
            CreateUserResponseDto userCreated = await _userService.GetUser(cpf);
            return Ok(userCreated);
        }
    }
}
