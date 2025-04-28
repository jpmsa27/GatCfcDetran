using GatCfcDetran.Services.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Interface
{
    public interface IUserService
    {
        Task<CreateUserResponseDto> CreateUser(CreateUserRequestDto requestDto, string cfcId);
        Task<CreateUserResponseDto> GetUser(string cpf);
    }
}
