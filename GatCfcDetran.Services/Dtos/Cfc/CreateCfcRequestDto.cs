using GatCfcDetran.SystemInfra.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Dtos.Cfc
{
    public class CreateCfcRequestDto
    {
        public required string Name { get; set; }
        public required string Cnpj { get; set; }
        public required string Address { get; set; }
        public required string Email { get; set; }
        public required string Cpf { get; set; }
        public required string Password { get; set; }

        public static implicit operator CreateCfcRequestDto(CfcEntity cfc)
        {
            return new CreateCfcRequestDto
            {
                Cnpj = cfc.Cnpj,
                Name = cfc.Name,
                Address = cfc.Address,
                Email = cfc.Email,
                Cpf = string.Empty,
                Password = string.Empty
            };
        }
    }
}
