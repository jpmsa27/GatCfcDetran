using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using GatCfcDetran.SystemInfra.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Bogus.Extensions.Brazil;
using GatCfcDetran.Services.Dtos.User;
using GatCfcDetran.Services.ExceptionUtils;
using GatCfcDetran.Services.Services;
using GatCfcDetran.SystemInfra.DataContext;

namespace GatCfcDetran.Tests.Testers
{
    public class UserServiceTests
    {
        private readonly DataContextDb _dbContext;
        private readonly UserService _userService; // supondo que se chama UserService
        private readonly Faker _faker;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<DataContextDb>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _dbContext = new DataContextDb(options);
            _userService = new UserService(_dbContext);
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task CreateUser_WithValidData_ShouldCreateUser()
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

            var requestDto = new CreateUserRequestDto
            {
                Cpf = _faker.Person.Cpf(),
                Password = _faker.Internet.Password(),
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                BirthDate = _faker.Date.Past(20)
            };

            // Act
            var response = await _userService.CreateUser(requestDto, cfc.Id);

            // Assert
            response.Should().NotBeNull();
            response.Cpf.Should().Be(requestDto.Cpf);
            response.Email.Should().Be(requestDto.Email);
            response.Name.Should().Be(requestDto.Name);
            response.BirthDate.Date.Should().Be(requestDto.BirthDate.Date);

            var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Cpf == requestDto.Cpf);
            userInDb.Should().NotBeNull();
            userInDb!.CfcId.Should().Be(cfc.Id);
        }

        [Fact]
        public async Task CreateUser_WhenUserAlreadyExists_ShouldThrowCustomException()
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

            var user = new UserEntity
            {
                Id = Guid.NewGuid().ToString(),
                Cpf = "12345678901",
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Password = _faker.Internet.Password(),
                CfcId = cfc.Id,
                Cfc = cfc,
                BirthDate = _faker.Date.Past(20),
                Role = SystemInfra.Enum.UserRole.USER,
                RegistrationId = Guid.NewGuid().ToString()
            };

            await _dbContext.Cfcs.AddAsync(cfc);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var requestDto = new CreateUserRequestDto
            {
                Cpf = user.Cpf, // mesmo CPF para forçar conflito
                Password = _faker.Internet.Password(),
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                BirthDate = _faker.Date.Past(20)
            };

            // Act
            Func<Task> act = async () => await _userService.CreateUser(requestDto, cfc.Id);

            // Assert
            await act.Should()
                .ThrowAsync<CustomException>()
                .WithMessage(CustomExceptionMessage.AlreadyExists)
                .Where(ex => ex.StatusCode == System.Net.HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateUser_WhenCfcNotFound_ShouldThrowCustomException()
        {
            // Arrange
            var fakeCfcId = Guid.NewGuid().ToString();

            var requestDto = new CreateUserRequestDto
            {
                Cpf = _faker.Person.Cpf(false),
                Password = _faker.Internet.Password(),
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                BirthDate = _faker.Date.Past(20)
            };

            // Act
            Func<Task> act = async () => await _userService.CreateUser(requestDto, fakeCfcId);

            // Assert
            await act.Should()
                .ThrowAsync<CustomException>()
                .WithMessage(CustomExceptionMessage.CfcNotFound)
                .Where(ex => ex.StatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetUser_WithExistingCpf_ShouldReturnUser()
        {
            var cfc = new CfcEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = _faker.Company.CompanyName(),
                Cnpj = _faker.Company.Cnpj(),
                Address = _faker.Address.FullAddress(),
                Email = _faker.Internet.Email()
            };
            // Arrange
            var user = new UserEntity
            {
                Id = Guid.NewGuid().ToString(),
                Cpf = _faker.Person.Cpf(),
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Password = _faker.Internet.Password(),
                CfcId = Guid.NewGuid().ToString(),
                Cfc = cfc,
                BirthDate = _faker.Date.Past(25),
                Role = SystemInfra.Enum.UserRole.USER,
                RegistrationId = Guid.NewGuid().ToString()
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var response = await _userService.GetUser(user.Cpf);

            // Assert
            response.Should().NotBeNull();
            response.Cpf.Should().Be(user.Cpf);
            response.Name.Should().Be(user.Name);
            response.Email.Should().Be(user.Email);
            response.BirthDate.Date.Should().Be(user.BirthDate.Date);
        }

        [Fact]
        public async Task GetUser_WithNonExistingCpf_ShouldThrowCustomException()
        {
            // Arrange
            var cpf = _faker.Person.Cpf();

            // Act
            Func<Task> act = async () => await _userService.GetUser(cpf);

            // Assert
            await act.Should()
                .ThrowAsync<CustomException>()
                .WithMessage(CustomExceptionMessage.UserNotFound)
                .Where(ex => ex.StatusCode == System.Net.HttpStatusCode.NotFound);
        }
    }
}
