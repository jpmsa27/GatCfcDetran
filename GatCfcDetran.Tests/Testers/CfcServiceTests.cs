using System;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using GatCfcDetran.SystemInfra.Entities;
using Bogus.Extensions.Brazil;
using GatCfcDetran.Services.Dtos.Cfc;
using GatCfcDetran.Services.ExceptionUtils;
using GatCfcDetran.SystemInfra.DataContext;
using GatCfcDetran.Services.Services;
using GatCfcDetran.Tests.BogusService;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GatCfcDetran.Tests.Testers
{
    public class CfcServiceTests
    {
        private readonly DataContextDb _dbContext;
        private readonly CfcServices _cfcService;
        private readonly Faker _faker;

        public CfcServiceTests()
        {
            var options = new DbContextOptionsBuilder<DataContextDb>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _dbContext = new DataContextDb(options);
            _cfcService = new CfcServices(_dbContext); // Supondo que o serviço se chama CfcService
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task CreateCfc_WithValidData_ShouldCreateCfcAndUser()
        {
            // Arrange
            var requestDto = EntityFakers.CfcEntityFaker.Generate();

            var cfcId = Guid.NewGuid().ToString(); // CfcId simulado

            var cfcDto = new CreateCfcRequestDto
            {
                Cnpj = requestDto.Cnpj,
                Name = requestDto.Name,
                Address = requestDto.Address,
                Email = requestDto.Email,
                Cpf = _faker.Person.Cpf(),
                Password = _faker.Internet.Password()
            };
            // Act
            var response = await _cfcService.CreateCfc(cfcDto, cfcId);

            // Assert
            response.Should().NotBeNull();
            response.Cnpj.Should().Be(requestDto.Cnpj);
            response.Name.Should().Be(requestDto.Name);
            response.Address.Should().Be(requestDto.Address);
            response.Email.Should().Be(requestDto.Email);

            var cfcInDb = await _dbContext.Cfcs.FirstOrDefaultAsync(x => x.Cnpj == requestDto.Cnpj);
            cfcInDb.Should().NotBeNull();

            var userInDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Cpf == cfcDto.Cpf && x.CfcId == cfcInDb!.Id);
            userInDb.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateCfc_WhenCfcAlreadyExists_ShouldThrowCustomException()
        {
            // Arrange
            var cnpj = _faker.Company.Cnpj();

            var existingCfc = new CfcEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = _faker.Company.CompanyName(),
                Cnpj = cnpj,
                Address = _faker.Address.FullAddress(),
                Email = _faker.Internet.Email()
            };

            await _dbContext.Cfcs.AddAsync(existingCfc);
            await _dbContext.SaveChangesAsync();

            var requestDto = EntityFakers.CfcEntityFaker.Generate();

            var cfcDto = new CreateCfcRequestDto
            {
                Cnpj = cnpj,
                Name = requestDto.Name,
                Address = requestDto.Address,
                Email = requestDto.Email,
                Cpf = _faker.Person.Cpf(),
                Password = _faker.Internet.Password()
            };

            var cfcId = Guid.NewGuid().ToString();

            // Act
            Func<Task> act = async () => await _cfcService.CreateCfc(cfcDto, cfcId);

            // Assert
            await act.Should()
                .ThrowAsync<CustomException>()
                .WithMessage(CustomExceptionMessage.AlreadyExists)
                .Where(ex => ex.StatusCode == System.Net.HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateCfc_WhenUserAlreadyExists_ShouldThrowCustomException()
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
                Cpf = "12345678901",
                Email = _faker.Internet.Email(),
                Password = _faker.Internet.Password(),
                Name = _faker.Person.FullName,
                CfcId = cfc.Id,
                Cfc = cfc,
                BirthDate = DateTime.UtcNow,
                Role = SystemInfra.Enum.UserRole.USER,
                RegistrationId = Guid.NewGuid().ToString()
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var requestDto = new CreateCfcRequestDto
            {
                Cnpj = _faker.Company.Cnpj(),
                Name = _faker.Company.CompanyName(),
                Address = _faker.Address.FullAddress(),
                Email = _faker.Internet.Email(),
                Cpf = "12345678901", // Mesmo CPF do usuário já existente
                Password = _faker.Internet.Password()
            };

            // Act
            Func<Task> act = async () => await _cfcService.CreateCfc(requestDto, cfc.Id);

            // Assert
            await act.Should()
                .ThrowAsync<CustomException>()
                .WithMessage(CustomExceptionMessage.AlreadyExists)
                .Where(ex => ex.StatusCode == System.Net.HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task GetCfc_WithExistingCnpj_ShouldReturnCfc()
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

            // Act
            var response = await _cfcService.GetCfc(cfc.Cnpj);

            // Assert
            response.Should().NotBeNull();
            response.Cnpj.Should().Be(cfc.Cnpj);
            response.Name.Should().Be(cfc.Name);
            response.Address.Should().Be(cfc.Address);
            response.Email.Should().Be(cfc.Email);
        }

        [Fact]
        public async Task GetCfc_WithNonExistingCnpj_ShouldThrowCustomException()
        {
            // Arrange
            var nonExistingCnpj = _faker.Company.Cnpj();

            // Act
            Func<Task> act = async () => await _cfcService.GetCfc(nonExistingCnpj);

            // Assert
            await act.Should()
                .ThrowAsync<CustomException>()
                .WithMessage(CustomExceptionMessage.CfcNotFound)
                .Where(ex => ex.StatusCode == System.Net.HttpStatusCode.NotFound);
        }
    }
}
