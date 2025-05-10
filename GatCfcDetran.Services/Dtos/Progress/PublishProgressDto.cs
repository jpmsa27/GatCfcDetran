using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Dtos.Progress
{
    public class PublishProgressDto
    {
        public required string Nome { get; set; }
        public required string Message { get; set; }
        public required string Email { get; set; }
        public required string Titulo { get; set; }
    }
}
