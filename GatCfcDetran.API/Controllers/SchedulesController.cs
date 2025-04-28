using GatCfcDetran.Services.Dtos.Schedule;
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
    public class SchedulesController(IScheduleService scheduleService) : ControllerBase
    {
        private readonly IScheduleService _scheduleService = scheduleService;

        [HttpPost]
        [Authorize(Policy = "CfcAdminPolicy")]
        [ProducesResponseType(typeof(RegisterScheduleResponseDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> RegisterSchedule(RegisterScheduleRequestDto requestDto)
        {
            var cfcId = User.Claims.FirstOrDefault(x => x.Type == "cfcId")!.Value;
            RegisterScheduleResponseDto scheduleCreated = await _scheduleService.RegisterSchedule(requestDto, cfcId);
            return Created($"api/schedules/{scheduleCreated.Id}", scheduleCreated);
        }

        [HttpGet("")]
        [Authorize(Policy = "UserAuthenticated")]
        [ProducesResponseType(typeof(List<RegisterScheduleResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersSchedules()
        {
            var cfcId = User.Claims.FirstOrDefault(x => x.Type == "cfcId")!.Value;
            var userId = User.Claims.FirstOrDefault(x => x.Type == "userId")!.Value;
            List<RegisterScheduleResponseDto> schedules = await _scheduleService.GetSchedules(userId ,cfcId);
            return Ok(schedules);
        }
        
        [HttpGet("{cpf}")]
        [Authorize(Policy = "UserAuthenticated")]
        [ProducesResponseType(typeof(List<RegisterScheduleResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserSchedules(string cpf)
        {
            var cfcId = User.Claims.FirstOrDefault(x => x.Type == "cfcId")!.Value;
            List<RegisterScheduleResponseDto> scheduleCreated = await _scheduleService.GetUserSchedules(cpf, cfcId);
            return Ok(scheduleCreated);
        }
    }
}
