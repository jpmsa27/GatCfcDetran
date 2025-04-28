using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using GatCfcDetran.SystemInfra.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Bogus.Extensions.Brazil;
using GatCfcDetran.Services.Dtos.Schedule;
using GatCfcDetran.Services.ExceptionUtils;
using GatCfcDetran.Services.Services;
using GatCfcDetran.SystemInfra.DataContext;
using GatCfcDetran.Services.ExternInterface;
using GatCfcDetran.Services.ExternServices;
using RabbitMQ.Client;

namespace GatCfcDetran.Tests.Testers
{
    public class ScheduleServiceTests
    {
        private readonly DataContextDb _dbContext;
        private readonly ScheduleService _scheduleService;
        private readonly Faker _faker;
        private readonly IRabbitService _rabbitService;

        public ScheduleServiceTests()
        {
            var options = new DbContextOptionsBuilder<DataContextDb>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _dbContext = new DataContextDb(options);
            _rabbitService = new RabbitService(new ConnectionFactory());
            _scheduleService = new ScheduleService(_dbContext, _rabbitService); // Supondo que o serviço se chama ScheduleService
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task GetUserSchedules_WithValidCpfAndCfcId_ShouldReturnSchedules()
        {
            // Arrange
            var cfcId = Guid.NewGuid().ToString();
            var cpf = _faker.Person.Cpf();

            var user = new UserEntity
            {
                Id = Guid.NewGuid().ToString(),
                Cpf = cpf,
                Email = _faker.Internet.Email(),
                Password = _faker.Internet.Password(),
                Name = _faker.Person.FullName,
                CfcId = cfcId,
                Cfc = new CfcEntity
                {
                    Id = cfcId,
                    Name = _faker.Company.CompanyName(),
                    Cnpj = _faker.Company.Cnpj(),
                    Address = _faker.Address.FullAddress(),
                    Email = _faker.Internet.Email()
                },
                BirthDate = _faker.Date.Past(30),
                Role = SystemInfra.Enum.UserRole.USER,
                RegistrationId = Guid.NewGuid().ToString()
            };

            await _dbContext.Users.AddAsync(user);

            var schedule = new ScheduleEntity
            {
                Id = Guid.NewGuid().ToString(),
                ScheduleDate = DateTime.UtcNow.AddDays(2),
                UserId = user.Id,
                Done = false
            };

            await _dbContext.Schedules.AddAsync(schedule);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _scheduleService.GetUserSchedules(cpf, cfcId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].ScheduleDate.Should().Be(schedule.ScheduleDate);
        }

        [Fact]
        public async Task GetUserSchedules_WithInvalidUser_ShouldThrowCustomException()
        {
            // Arrange
            var cpf = _faker.Person.Cpf();
            var cfcId = Guid.NewGuid().ToString();

            // Act
            Func<Task> act = async () => await _scheduleService.GetUserSchedules(cpf, cfcId);

            // Assert
            await act.Should()
                .ThrowAsync<CustomException>()
                .WithMessage(CustomExceptionMessage.UserNotFound)
                .Where(ex => ex.StatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task RegisterSchedule_WithValidData_ShouldCreateScheduleAndUserProgress()
        {
            // Arrange
            var cfc = new CfcEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = _faker.Company.CompanyName(),
                Cnpj = _faker.Company.Cnpj(),
                Address = _faker.Address.FullAddress(),
                Email = _faker.Internet.Email()
            };

            await _dbContext.Cfcs.AddAsync(cfc);

            var user = new UserEntity
            {
                Id = Guid.NewGuid().ToString(),
                Cpf = _faker.Person.Cpf(),
                Email = _faker.Internet.Email(),
                Password = _faker.Internet.Password(),
                Name = _faker.Person.FullName,
                CfcId = cfc.Id,
                Cfc = cfc,
                BirthDate = _faker.Date.Past(30),
                Role = SystemInfra.Enum.UserRole.USER,
                RegistrationId = Guid.NewGuid().ToString()
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var requestDto = new RegisterScheduleRequestDto
            {
                Cpf = user.Cpf,
                ScheduleDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var response = await _scheduleService.RegisterSchedule(requestDto, cfc.Id);

            // Assert
            response.Should().NotBeNull();
            response.ScheduleDate.Should().Be(requestDto.ScheduleDate);

            var scheduleInDb = await _dbContext.Schedules.FirstOrDefaultAsync(s => s.UserId == user.Id);
            scheduleInDb.Should().NotBeNull();

            var userProgress = await _dbContext.UsersProgress.FirstOrDefaultAsync(up => up.UserId == user.Id);
            userProgress.Should().NotBeNull();
            userProgress.AulasTotais.Should().Be(1);
        }

        [Fact]
        public async Task RegisterSchedule_WhenUserNotFound_ShouldThrowCustomException()
        {
            // Arrange
            var cfc = new CfcEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = _faker.Company.CompanyName(),
                Cnpj = _faker.Company.Cnpj(),
                Address = _faker.Address.FullAddress(),
                Email = _faker.Internet.Email()
            };

            await _dbContext.Cfcs.AddAsync(cfc);
            await _dbContext.SaveChangesAsync();

            var requestDto = new RegisterScheduleRequestDto
            {
                Cpf = _faker.Person.Cpf(),
                ScheduleDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            Func<Task> act = async () => await _scheduleService.RegisterSchedule(requestDto, cfc.Id);

            // Assert
            await act.Should()
                .ThrowAsync<CustomException>()
                .WithMessage(CustomExceptionMessage.UserNotFound)
                .Where(ex => ex.StatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task RegisterSchedule_WhenCfcNotFound_ShouldThrowCustomException()
        {
            // Arrange
            var requestDto = new RegisterScheduleRequestDto
            {
                Cpf = _faker.Person.Cpf(),
                ScheduleDate = DateTime.UtcNow.AddDays(1)
            };

            var fakeCfcId = Guid.NewGuid().ToString(); // CfcId que não existe

            // Act
            Func<Task> act = async () => await _scheduleService.RegisterSchedule(requestDto, fakeCfcId);

            // Assert
            await act.Should()
                .ThrowAsync<CustomException>()
                .WithMessage(CustomExceptionMessage.CfcNotFound)
                .Where(ex => ex.StatusCode == System.Net.HttpStatusCode.NotFound);
        }
    }
}
