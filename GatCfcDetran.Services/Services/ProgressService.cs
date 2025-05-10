using GatCfcDetran.Services.Dtos.Progress;
using GatCfcDetran.Services.ExceptionUtils;
using GatCfcDetran.Services.Interface;
using GatCfcDetran.SystemInfra.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Services
{
    public class ProgressService(DataContextDb dataContextDb) : IProgressService
    {
        private readonly DataContextDb _dataContextDb = dataContextDb;
        public async Task<ProgressResponseDto> GetProgress(string userCpf, string cfcId)
        {
            var user = await _dataContextDb.Users.FirstOrDefaultAsync(x => x.Cpf == userCpf && x.CfcId == cfcId)
                ?? throw new CustomException(CustomExceptionMessage.UserNotFound, System.Net.HttpStatusCode.NotFound);

            var progress = await _dataContextDb.UsersProgress.FirstOrDefaultAsync(x => x.UserId == user.Id)
                ?? throw new CustomException(CustomExceptionMessage.ProgressNotFound, System.Net.HttpStatusCode.NotFound);

            var progressResponse = new ProgressResponseDto
            {
                AulasMinimas = progress.AulasMinimas,
                AulasTotais = progress.AulasTotais,
                Id = progress.Id,
            };

            return progressResponse;
        }

    }
}
