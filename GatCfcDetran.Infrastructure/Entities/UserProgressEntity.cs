using GatCfcDetran.SystemInfra.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.SystemInfra.Entities
{
    public class UserProgressEntity : Entity
    {
        public int AulasTotais {  get; set; }
        public int AulasMinimas { get; set; }
        public string? UserId { get; set; }
        public virtual UserEntity? User { get; set; }
    }
}
