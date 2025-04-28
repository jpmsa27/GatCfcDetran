using GatCfcDetran.Services.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Interface
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Auth(AuthRequestDto authRequestDto);
    }
}
