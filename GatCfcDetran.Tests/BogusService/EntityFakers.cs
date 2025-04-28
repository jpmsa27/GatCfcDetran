using Bogus;
using Bogus.Extensions.Brazil;
using GatCfcDetran.SystemInfra.Entities;
using GatCfcDetran.SystemInfra.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Tests.BogusService
{
    public static class EntityFakers
    {
        public static Faker<CfcEntity> CfcEntityFaker => new Faker<CfcEntity>("pt_BR")
        .RuleFor(c => c.Id, f => f.Random.Guid().ToString())
        .RuleFor(c => c.Name, f => f.Company.CompanyName())
        .RuleFor(c => c.Cnpj, f => f.Company.Cnpj())
        .RuleFor(c => c.Address, f => f.Address.FullAddress())
        .RuleFor(c => c.Email, f => f.Internet.Email());

        public static Faker<UserEntity> UserEntityFaker => new Faker<UserEntity>("pt_BR")
            .RuleFor(u => u.Id, f => f.Random.Guid().ToString())
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.Cpf, f => f.Person.Cpf())
            .RuleFor(u => u.Password, f => f.Internet.Password(8, true)) // Senha com 8 caracteres, incluindo letras, números e símbolos
            .RuleFor(u => u.BirthDate, f => f.Date.Past(30, DateTime.Today.AddYears(-18))) // Entre 18 e 48 anos
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.RegistrationId, f => f.Random.AlphaNumeric(10))
            .RuleFor(u => u.Role, f => f.PickRandom<UserRole>())
            .RuleFor(u => u.CfcId, f => f.Random.Guid().ToString())
            .RuleFor(u => u.Cfc, f => CfcEntityFaker.Generate());

        public static Faker<ScheduleEntity> ScheduleEntityFaker => new Faker<ScheduleEntity>("pt_BR")
            .RuleFor(s => s.Id, f => f.Random.Guid().ToString())
            .RuleFor(s => s.ScheduleDate, f => f.Date.Future())
            .RuleFor(s => s.UserId, f => f.Random.Guid().ToString())
            .RuleFor(s => s.User, f => UserEntityFaker.Generate())
            .RuleFor(s => s.Done, f => f.Random.Bool());

        public static Faker<UserProgressEntity> UserProgressEntityFaker => new Faker<UserProgressEntity>("pt_BR")
            .RuleFor(p => p.Id, f => f.Random.Guid().ToString())
            .RuleFor(p => p.AulasTotais, f => f.Random.Int(10, 50))
            .RuleFor(p => p.AulasMinimas, (f, p) => f.Random.Int(5, p.AulasTotais)) // Aulas mínimas <= totais
            .RuleFor(p => p.UserId, f => f.Random.Guid().ToString())
            .RuleFor(p => p.User, f => UserEntityFaker.Generate());
    }
}
