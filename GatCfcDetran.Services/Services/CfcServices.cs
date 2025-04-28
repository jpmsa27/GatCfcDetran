using GatCfcDetran.Services.Dtos.Cfc;
using GatCfcDetran.Services.ExceptionUtils;
using GatCfcDetran.Services.Interface;
using GatCfcDetran.SystemInfra.DataContext;
using GatCfcDetran.SystemInfra.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Services
{
    public class CfcServices(DataContextDb dbContext) : ICfcServices
    {
        private readonly DataContextDb _dbContext = dbContext;

        public async Task<CreateCfcResponseDto> CreateCfc(CreateCfcRequestDto requestDto, string cfcId)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            var cfcExists = await _dbContext.Cfcs.FirstOrDefaultAsync(x => x.Cnpj == requestDto.Cnpj);

            if (cfcExists != null)
            {
                throw new CustomException(CustomExceptionMessage.AlreadyExists, System.Net.HttpStatusCode.Conflict);
            }

            var cfc = new CfcEntity
            {
                Cnpj = requestDto.Cnpj,
                Name = requestDto.Name,
                Address = requestDto.Address,
                Email = requestDto.Email
            };

            await _dbContext.Cfcs.AddAsync(cfc);

            var userExists = await _dbContext.Users.FirstOrDefaultAsync(x => x.Cpf == requestDto.Cpf && x.CfcId == cfcId);

            if (userExists != null)
            {
                throw new CustomException(CustomExceptionMessage.AlreadyExists, System.Net.HttpStatusCode.Conflict);
            }

            var user = new UserEntity
            {
                Cpf = requestDto.Cpf,
                Password = requestDto.Password,
                Name = requestDto.Name,
                Email = requestDto.Email,
                CfcId = cfc.Id,
                Cfc = cfc,
                BirthDate = DateTime.UtcNow,
                Role = SystemInfra.Enum.UserRole.USER,
                RegistrationId = Guid.NewGuid().ToString()
            };

            await _dbContext.Users.AddAsync(user);

            try
            {
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            (Exception ex)
            {
                _ = ex;
                await transaction.RollbackAsync();
                throw new CustomException(CustomExceptionMessage.ErrorOnCreate, System.Net.HttpStatusCode.InternalServerError);
            }

            return new CreateCfcResponseDto
            {
                Id = cfc.Id,
                Name = cfc.Name,
                Cnpj = cfc.Cnpj,
                Address = cfc.Address,
                Email = cfc.Email
            };
        }

        public async Task<CreateCfcResponseDto> GetCfc(string cnpj)
        {
            var cfc = await _dbContext.Cfcs.FirstOrDefaultAsync(x => x.Cnpj == cnpj) ??
                throw new CustomException(CustomExceptionMessage.CfcNotFound, System.Net.HttpStatusCode.NotFound);

            return new CreateCfcResponseDto
            {
                Id = cfc.Id,
                Name = cfc.Name,
                Cnpj = cfc.Cnpj,
                Address = cfc.Address,
                Email = cfc.Email
            };
        }

    }
}
