using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Dtos.Schedule
{
    public class RegisterScheduleRequestDto
    {
        public required string Cpf { get; set; }
        public DateTime ScheduleDate { get; set; }
    }
}
