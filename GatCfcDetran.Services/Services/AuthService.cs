using GatCfcDetran.Services.Dtos.Auth;
using GatCfcDetran.Services.ExceptionUtils;
using GatCfcDetran.Services.Interface;
using GatCfcDetran.SystemInfra.DataContext;
using GatCfcDetran.SystemInfra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Services
{
    public class AuthService(DataContextDb contextDb, IConfiguration configuration) : IAuthService
    {
        private readonly DataContextDb _dbContext = contextDb;
        private readonly IConfiguration _configuration = configuration;

        public async Task<AuthResponseDto> Auth(AuthRequestDto authRequestDto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == authRequestDto.Email) ?? 
                throw new CustomException(CustomExceptionMessage.UserNotFound, System.Net.HttpStatusCode.NotFound);
            
            var token = GenerateToken(user);

            return new AuthResponseDto() { Token = token };
        }

        private string GenerateToken(UserEntity user)
        {
            var claims = new[] {
                new Claim ("cfcId", user.CfcId),
                new Claim ("userId", user.Id),
                new Claim ("role", user.Role.ToString())
            };
            var secret = _configuration.GetSection("JwtTokenData:Secret").Value;
            if (string.IsNullOrEmpty(secret))
            {
                throw new CustomException(CustomExceptionMessage.TokenNotFound, System.Net.HttpStatusCode.NotFound);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken token = new(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }
    }
}
