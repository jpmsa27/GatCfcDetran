using GatCfcDetran.Services.Dtos.Schedule;
using GatCfcDetran.SystemInfra.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Dtos.Auth
{
    public class AuthRequestDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        [MinLength(8)]
        public required string Password { get; set; }

        public static implicit operator AuthRequestDto(UserEntity user)
        {
            return new AuthRequestDto
            {
                Email = user.Email,
                Password = user.Password
            };
        }
    }
}
