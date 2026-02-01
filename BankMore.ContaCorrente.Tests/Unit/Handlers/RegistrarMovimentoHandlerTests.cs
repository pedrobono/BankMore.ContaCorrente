using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Xunit;
using BankMore.ContaCorrente.Application.Handlers;
using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Infrastructure.Data;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BankMore.ContaCorrente.Tests.Unit.Handlers
{
    public class RegistrarMovimentoHandlerTests
    {
        private readonly DataBaseContext _context;
        private readonly RegistrarMovimentoHandler _handler;

        public RegistrarMovimentoHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new DataBaseContext(options);
            _handler = new RegistrarMovimentoHandler(_context);
        }

        [Fact]
        public async Task Handle_ShouldRegisterMovement_WhenValidMovement()
        {
            // Arrange
            _context.ContaCorrente.Add(new Conta
            {
                IdContaCorrente = Guid.NewGuid(),
                Salt = "12345678901",
                Numero = 12345,
                Nome = "Test User",
                Senha = "hash",
                Ativo = 1
            });
            await _context.SaveChangesAsync();

            var command = new RegistrarMovimentoCommand
            {
                RequestId = Guid.NewGuid().ToString(),
                NumeroConta = "12345-5",
                Valor = 100m,
                Tipo = "C"
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            var idempotencia = await _context.Idempotencia.FirstOrDefaultAsync(i => i.ChaveIdempotencia == Guid.Parse(command.RequestId));
            Assert.NotNull(idempotencia);
        }

        [Fact]
        public async Task Handle_ShouldNotRegisterMovement_WhenInvalidAccount()
        {
            // Arrange
            var command = new RegistrarMovimentoCommand
            {
                RequestId = Guid.NewGuid().ToString(),
                NumeroConta = "99999-9",
                Valor = 100m,
                Tipo = "C"
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_IdempotentRequest_ShouldNotDuplicate()
        {
            // Arrange
            _context.ContaCorrente.Add(new Conta
            {
                IdContaCorrente = Guid.NewGuid(),
                Salt = "12345678901",
                Numero = 12345,
                Nome = "Test User",
                Senha = "hash",
                Ativo = 1
            });
            await _context.SaveChangesAsync();

            var requestId = Guid.NewGuid().ToString();
            var command = new RegistrarMovimentoCommand
            {
                RequestId = requestId,
                NumeroConta = "12345-5",
                Valor = 100m,
                Tipo = "C"
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            var idempotencias = await _context.Idempotencia.Where(i => i.ChaveIdempotencia == Guid.Parse(requestId)).ToListAsync();
            Assert.Single(idempotencias);
        }

        [Fact]
        public async Task Handle_InactiveAccount_ShouldThrowException()
        {
            // Arrange
            _context.ContaCorrente.Add(new Conta
            {
                IdContaCorrente = Guid.NewGuid(),
                Salt = "12345678901",
                Numero = 12345,
                Nome = "Test User",
                Senha = "hash",
                Ativo = 0
            });
            await _context.SaveChangesAsync();

            var command = new RegistrarMovimentoCommand
            {
                RequestId = Guid.NewGuid().ToString(),
                NumeroConta = "12345-5",
                Valor = 100m,
                Tipo = "C"
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidValue_WhenNegativeValue()
        {
            // Arrange
            _context.ContaCorrente.Add(new Conta
            {
                IdContaCorrente = Guid.NewGuid(),
                Salt = "12345678901",
                Numero = 12345,
                Nome = "Test User",
                Senha = "hash",
                Ativo = 1
            });
            await _context.SaveChangesAsync();

            var command = new RegistrarMovimentoCommand
            {
                RequestId = Guid.NewGuid().ToString(),
                NumeroConta = "12345-5",
                Valor = -100m,
                Tipo = "C"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() =>
                _handler.Handle(command, CancellationToken.None));
            Assert.Equal("INVALID_VALUE", exception.FailureType);
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidType_WhenInvalidTipo()
        {
            // Arrange
            _context.ContaCorrente.Add(new Conta
            {
                IdContaCorrente = Guid.NewGuid(),
                Salt = "12345678901",
                Numero = 12345,
                Nome = "Test User",
                Senha = "hash",
                Ativo = 1
            });
            await _context.SaveChangesAsync();

            var command = new RegistrarMovimentoCommand
            {
                RequestId = Guid.NewGuid().ToString(),
                NumeroConta = "12345-5",
                Valor = 100m,
                Tipo = "X"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() =>
                _handler.Handle(command, CancellationToken.None));
            Assert.Equal("INVALID_TYPE", exception.FailureType);
        }
    }
}
