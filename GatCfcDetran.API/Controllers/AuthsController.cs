using GatCfcDetran.Services.Dtos.Auth;
using GatCfcDetran.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatCfcDetran.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthsController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Auth(AuthRequestDto authRequestDto)
        {
            var ret = await _authService.Auth(authRequestDto);
            return Ok(ret);
        }
    }
}
