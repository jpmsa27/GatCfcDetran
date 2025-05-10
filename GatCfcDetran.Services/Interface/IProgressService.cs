using GatCfcDetran.Services.Dtos.Progress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Interface
{
    public interface IProgressService
    {
        Task<ProgressResponseDto> GetProgress(string userCpf, string cfcId);
    }
}
