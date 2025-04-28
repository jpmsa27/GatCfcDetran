using GatCfcDetran.Services.Dtos.Cfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Interface
{
    public interface ICfcServices
    {
        Task<CreateCfcResponseDto> CreateCfc(CreateCfcRequestDto requestDto, string cfcId);
        Task<CreateCfcResponseDto> GetCfc(string cnpj);
    }
}
