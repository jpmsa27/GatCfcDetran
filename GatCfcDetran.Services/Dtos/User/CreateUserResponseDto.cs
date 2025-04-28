using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Dtos.User
{
    public class CreateUserResponseDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Cpf { get; set; }
        public DateTime BirthDate { get; set; }
        public required string Email { get; set; }
    }
}
