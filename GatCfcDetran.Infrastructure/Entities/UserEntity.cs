using GatCfcDetran.SystemInfra.Entities.Base;
using GatCfcDetran.SystemInfra.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.SystemInfra.Entities
{
    public class UserEntity : Entity
    {
        public required string Name { get; set; }
        public required string Cpf { get; set; }
        public DateTime BirthDate { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string RegistrationId { get; set; }
        public UserRole Role { get; set; }
        public required string CfcId { get; set; }
        public required virtual CfcEntity Cfc { get; set; }
    }
}
