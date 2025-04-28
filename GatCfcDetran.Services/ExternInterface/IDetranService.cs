using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.ExternInterface
{
    public interface IDetranService
    {
        [Get("detran/aluno/{registerId}")]
        Task<HttpResponseMessage> GetUserProgress(string registerId);
    }
}
