using GatCfcDetran.SystemInfra.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Dtos.Schedule
{
    public class RegisterScheduleResponseDto
    {
        public required string Id { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string? UserId { get; set; }
        public bool Done { get; set; }
    
        public static implicit operator RegisterScheduleResponseDto(ScheduleEntity schedule)
        {
            var scheduleDto = new RegisterScheduleResponseDto
            {
                Id = schedule.Id,
                ScheduleDate = schedule.ScheduleDate,
                UserId = schedule.UserId,
                Done = schedule.Done
            };

            return scheduleDto;
        }
    }
}
