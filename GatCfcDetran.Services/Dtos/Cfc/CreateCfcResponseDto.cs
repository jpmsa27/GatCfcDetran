using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Dtos.Cfc
{
    public class CreateCfcResponseDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Cnpj { get; set; }
        public required string Address { get; set; }
        public required string Email { get; set; }
    }
}
