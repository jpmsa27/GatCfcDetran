using GatCfcDetran.Services.Dtos.User;
using GatCfcDetran.Services.ExceptionUtils;
using GatCfcDetran.Services.Interface;
using GatCfcDetran.SystemInfra.DataContext;
using GatCfcDetran.SystemInfra.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Services
{
    public class UserService(DataContextDb contextDb) : IUserService
    {
        private readonly DataContextDb _dbContext = contextDb;

        public async Task<CreateUserResponseDto> CreateUser(CreateUserRequestDto requestDto, string cfcId)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();

            var userExists = await _dbContext.Users.FirstOrDefaultAsync(x => x.Cpf == requestDto.Cpf && x.CfcId == cfcId);
            if (userExists != null) throw new CustomException(CustomExceptionMessage.AlreadyExists, System.Net.HttpStatusCode.Conflict);

            var cfcExists = await _dbContext.Cfcs.FirstOrDefaultAsync(x => x.Id == cfcId) ??
                throw new CustomException(CustomExceptionMessage.CfcNotFound, System.Net.HttpStatusCode.NotFound);
            
            var user = new UserEntity
            {
                Cpf = requestDto.Cpf,
                Password = requestDto.Password,
                Name = requestDto.Name,
                Email = requestDto.Email,
                CfcId = cfcExists.Id,
                Cfc = cfcExists,
                BirthDate = requestDto.BirthDate,
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

            return new CreateUserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Cpf = user.Cpf,
                Email = user.Email,
                BirthDate = user.BirthDate
            };
        }

        public async Task<CreateUserResponseDto> GetUser(string cpf)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Cpf == cpf) ??
                throw new CustomException(CustomExceptionMessage.UserNotFound, System.Net.HttpStatusCode.NotFound);


            return new CreateUserResponseDto() 
            { 
                Id = user.Id,
                Cpf = user.Cpf, 
                Name = user.Name, 
                Email = user.Email, 
                BirthDate = user.BirthDate
            };
        }
    }
}
