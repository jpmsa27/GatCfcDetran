using GatCfcDetran.Services.Dtos.Progress;
using GatCfcDetran.Services.Dtos.Schedule;
using GatCfcDetran.Services.ExceptionUtils;
using GatCfcDetran.Services.ExternInterface;
using GatCfcDetran.Services.Interface;
using GatCfcDetran.SystemInfra.DataContext;
using GatCfcDetran.SystemInfra.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Services
{
    public class ScheduleService(DataContextDb contextDb, IRabbitService rabbitService) : IScheduleService
    {
        private readonly DataContextDb _contextDb = contextDb;
        private readonly IRabbitService _rabbitService = rabbitService;

        public async Task<List<RegisterScheduleResponseDto>> GetSchedules(string userId,string cfcId)
        {
            var schedules = await _contextDb.Schedules.Where(x => x.UserId == userId).ToListAsync();

            var schedulesDto = schedules.Select(x => (RegisterScheduleResponseDto)x).ToList();

            return schedulesDto;
        }

        public async Task<List<RegisterScheduleResponseDto>> GetUserSchedules(string cpf, string cfcId)
        {
            var user = await _contextDb.Users.FirstOrDefaultAsync(x => x.Cpf == cpf && x.CfcId == cfcId) ?? 
                throw new CustomException(CustomExceptionMessage.UserNotFound, System.Net.HttpStatusCode.NotFound);
            
            var schedules = await _contextDb.Schedules.Where(x => x.UserId == user.Id).ToListAsync();

            var schedulesDto = schedules.Select(x => (RegisterScheduleResponseDto)x).ToList();
            
            return schedulesDto;
        }

        public async Task<RegisterScheduleResponseDto> RegisterSchedule(RegisterScheduleRequestDto requestDto, string cfcId)
        {
            var transaction = await _contextDb.Database.BeginTransactionAsync();

            var cfc = await _contextDb.Cfcs.FirstOrDefaultAsync(x => x.Id == cfcId) ??
                throw new CustomException(CustomExceptionMessage.CfcNotFound, System.Net.HttpStatusCode.NotFound);
            
            var user = await _contextDb.Users.FirstOrDefaultAsync(x => x.Cpf == requestDto.Cpf && x.CfcId == cfcId) ??
                throw new CustomException(CustomExceptionMessage.UserNotFound, System.Net.HttpStatusCode.NotFound);

            _ = cfc;

            var schedule = new ScheduleEntity
            {
                CreationDate = DateTime.UtcNow,
                ScheduleDate = requestDto.ScheduleDate,
                UserId = user.Id,
                User = user,
            };

            await _contextDb.Schedules.AddAsync(schedule);

            var userProgressExists = await _contextDb.UsersProgress.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (userProgressExists != null)
            {
                userProgressExists.AulasTotais = userProgressExists.AulasTotais ++;
                _contextDb.UsersProgress.Update(userProgressExists);

                if(userProgressExists.AulasTotais >= userProgressExists.AulasMinimas)
                {
                    var progressMessage = new PublishProgressDto() 
                    {
                        Titulo = "Exame Prático Disponível",
                        Email = user.Email, 
                        Message = "O usuário que recebeu essa mensagem já pode efetuar o exame prático.", 
                        Nome = user.Name 
                    };
                    await _rabbitService.PublishAsync(JsonSerializer.Serialize(progressMessage));
                }
            }
            else
            {
                var userProgress = new UserProgressEntity
                {
                    UserId = user.Id,
                    AulasTotais = 1,
                    User = user,
                    AulasMinimas = 10,
                };
                await _contextDb.UsersProgress.AddAsync(userProgress);
            }

            try
            {
                await _contextDb.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            (Exception ex)
            {
                _ = ex;
                await transaction.RollbackAsync();
                throw new CustomException(CustomExceptionMessage.ErrorOnCreate, System.Net.HttpStatusCode.InternalServerError);
            }

            await _rabbitService.PublishAsync(JsonSerializer.Serialize(new PublishProgressDto()
            {
                Titulo = "Exame marcado com sucesso!",
                Email = user.Email,
                Message = $"Seu exame foi marcado com sucesso para o dia: { requestDto.ScheduleDate }",
                Nome = user.Name
            }));
            return (RegisterScheduleResponseDto)schedule;
        }
    }
}
