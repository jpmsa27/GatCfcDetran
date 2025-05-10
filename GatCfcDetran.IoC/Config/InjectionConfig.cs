using GatCfcDetran.Services.ExternInterface;
using GatCfcDetran.SystemInfra.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Refit;
using System.Net.Mail;
using GatCfcDetran.Services.Interface;
using GatCfcDetran.Services.Services;
using RabbitMQ.Client;
using GatCfcDetran.Services.ExternServices;
using GatCfcDetran.Services.BackgroundServices;

namespace GatCfcDetran.IoC.Config
{
    public static class InjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<DataContextDb>(options =>
               options.UseNpgsql(configuration.GetSection("ConnectionStrings:Database").Value));

            string apiString = configuration.GetSection("ConnectionStrings:Detran").Value ?? "";

            services.AddRefitClient<IDetranService>().
                ConfigureHttpClient(c => c.BaseAddress = new Uri(apiString));

            var host = configuration.GetSection("SMTPCredentials:Host").Value;
            var password = configuration.GetSection("SMTPCredentials:SenhaDeApp").Value;
            var user = configuration.GetSection("SMTPCredentials:User").Value;
            services.AddTransient(provider =>
            {
                var smtpClient = new SmtpClient(host, 587)
                {
                    Credentials = new System.Net.NetworkCredential(user, password),
                    EnableSsl = true
                };
                return smtpClient;
            });

            services.AddSingleton(serviceProvider =>
            {
                return new ConnectionFactory
                {
                    Uri = new Uri(configuration.GetSection("RabbitConfig:Uri").Value!),
                    UserName = configuration.GetSection("RabbitConfig:UserName").Value!,
                    Password = configuration.GetSection("RabbitConfig:Password").Value!
                };
            });

            services.AddSingleton<RabbitService>();
            services.AddScoped<DbContext, DataContextDb>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProgressService, ProgressService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<ICfcServices, CfcServices>();
            services.AddSingleton<IRabbitService, RabbitService>();
            services.AddSingleton<IEmailService, EmailService>();

            services.AddHostedService<EmailBackgroundService>();

            return services;
        }
    }
}
