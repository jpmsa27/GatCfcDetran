using System;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using GatCfcDetran.SystemInfra.Entities;
using GatCfcDetran.Services.Dtos.Auth;
using GatCfcDetran.Services.ExceptionUtils;
using GatCfcDetran.Services.Services;
using GatCfcDetran.SystemInfra.DataContext;
using GatCfcDetran.Tests.BogusService;

namespace GatCfcDetran.Tests.Testers
{
    public class AuthServiceTests
    {
        private readonly DataContextDb _dbContext;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;
        private readonly Faker _faker;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<DataContextDb>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new DataContextDb(options);
            _configurationMock = new Mock<IConfiguration>();
            _authService = new AuthService(_dbContext, _configurationMock.Object);

            _faker = new Faker();
        }

        [Fact]
        public async Task Auth_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var userFaker = EntityFakers.UserEntityFaker;

            var user = userFaker.Generate();

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            _configurationMock.Setup(c => c.GetSection("JwtTokenData:Secret").Value)
                .Returns(_faker.Random.AlphaNumeric(32)); // Token secreto aleatório

            // Act
            var result = await _authService.Auth(user);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Auth_WithInvalidCredentials_ShouldThrowCustomException()
        {
            // Arrange
            var authRequestDtoFaker = new Faker<AuthRequestDto>()
                .RuleFor(a => a.Email, f => f.Internet.Email()); // email que não existe no banco

            var authRequest = authRequestDtoFaker.Generate();

            _configurationMock.Setup(c => c.GetSection("Token:secret").Value)
                .Returns(_faker.Random.AlphaNumeric(32));

            // Act
            Func<Task> act = async () => await _authService.Auth(authRequest);

            // Assert
            await act.Should()
                .ThrowAsync<CustomException>()
                .WithMessage(CustomExceptionMessage.UserNotFound)
                .Where(ex => ex.StatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Auth_WhenTokenSecretIsMissing_ShouldThrowCustomException()
        {
            // Arrange
            var userFaker = EntityFakers.UserEntityFaker;

            var user = userFaker.Generate();

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var authRequestDtoFaker = new Faker<AuthRequestDto>()
                .RuleFor(a => a.Email, f => user.Email);

            var authRequest = authRequestDtoFaker.Generate();

            _configurationMock.Setup(c => c.GetSection("JwtTokenData:Secret").Value)
                .Returns(""); 

            // Act
            Func<Task> act = async () => await _authService.Auth(authRequest);

            // Assert
            await act.Should()
                .ThrowAsync<CustomException>()
                .WithMessage(CustomExceptionMessage.TokenNotFound)
                .Where(ex => ex.StatusCode == System.Net.HttpStatusCode.NotFound);
        }
    }
}
