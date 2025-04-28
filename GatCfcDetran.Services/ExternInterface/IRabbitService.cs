using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.ExternInterface
{
    public interface IRabbitService
    {
        Task PublishAsync(string message);
        Task<string?> ConsumeMessageAsync();
    }
}
