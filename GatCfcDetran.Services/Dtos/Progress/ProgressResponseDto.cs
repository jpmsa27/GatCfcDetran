using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.Dtos.Progress
{
    public class ProgressResponseDto
    {
        public required string Id { get; set; }
        public int AulasTotais { get; set; }
        public int AulasMinimas { get; set; }
    }
}
