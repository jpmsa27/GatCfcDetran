using GatCfcDetran.SystemInfra.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.SystemInfra.Entities
{
    public class ScheduleEntity : Entity
    {
        public DateTime ScheduleDate { get; set; }
        public string? UserId { get; set; }
        public virtual UserEntity? User { get; set; }
        public bool Done { get; set; }
    }
}
