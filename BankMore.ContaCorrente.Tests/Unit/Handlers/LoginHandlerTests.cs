using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;
using BankMore.ContaCorrente.Application.Handlers;
using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Infrastructure.Data;
using BankMore.ContaCorrente.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BankMore.ContaCorrente.Tests.UnitTests
{
    public class LoginHandlerTests
    {
        private readonly DataBaseContext _context;
        private readonly Mock<IConfiguration> _configMock;
        private readonly LoginHandler _handler;

        public LoginHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new DataBaseContext(options);
            
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(c => c["JwtSettings:SecretKey"]).Returns("TestSecretKeyForJWT123456789012345");
            
            _handler = new LoginHandler(_context, _configMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsToken()
        {
            // Arrange
            _context.Contas.Add(new Conta
            {
                Id = Guid.NewGuid(),
                Cpf = "12345678901",
                NumeroConta = "12345-6",
                NomeTitular = "Test User",
                Senha = BCrypt.Net.BCrypt.HashPassword("password123"),
                Ativa = true
            });
            await _context.SaveChangesAsync();

            var command = new LoginCommand { CpfOrNumeroConta = "12345678901", Senha = "password123" };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result.Token));
        }

        [Fact]
        public async Task Handle_InvalidCredentials_ThrowsUnauthorizedException()
        {
            // Arrange
            _context.Contas.Add(new Conta
            {
                Id = Guid.NewGuid(),
                Cpf = "12345678901",
                NumeroConta = "12345-6",
                NomeTitular = "Test User",
                Senha = BCrypt.Net.BCrypt.HashPassword("password123"),
                Ativa = true
            });
            await _context.SaveChangesAsync();

            var command = new LoginCommand { CpfOrNumeroConta = "12345678901", Senha = "wrongpassword" };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
