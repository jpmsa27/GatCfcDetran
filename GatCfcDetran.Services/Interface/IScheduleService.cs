using GatCfcDetran.Services.Dtos.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Interface
{
    public interface IScheduleService
    {
        Task<List<RegisterScheduleResponseDto>> GetSchedules(string userId,string cfcId);
        Task<List<RegisterScheduleResponseDto>> GetUserSchedules(string cpf, string cfcId);
        Task<RegisterScheduleResponseDto> RegisterSchedule(RegisterScheduleRequestDto requestDto, string cfcId);
    }
}
