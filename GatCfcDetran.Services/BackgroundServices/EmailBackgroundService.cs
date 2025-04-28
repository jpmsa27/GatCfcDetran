using GatCfcDetran.Services.Dtos.Progress;
using GatCfcDetran.Services.ExternInterface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.BackgroundServices
{
    public class EmailBackgroundService(IRabbitService rabbitMqService, IEmailService emailService) : BackgroundService
    {
        private readonly IRabbitService _rabbitMqService = rabbitMqService;
        private readonly IEmailService _emailService = emailService;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _rabbitMqService.ConsumeMessageAsync();

                if (!string.IsNullOrEmpty(message))
                {
                    var mensagemObj = JsonSerializer.Deserialize<PublishProgressDto>(message);

                    var destinatario = mensagemObj!.Email;
                    await _emailService.SendEmailAsync(destinatario, "Mensagem de conclusão mínima de exames!", message);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Espera 1 minuto
            }
        }
    }
}
