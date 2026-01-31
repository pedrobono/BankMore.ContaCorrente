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
            _context.Contas.Add(new Conta
            {
                Id = Guid.NewGuid(),
                Cpf = "12345678901",
                NumeroConta = "12345-6",
                NomeTitular = "Test User",
                Senha = "hash",
                Ativa = true
            });
            await _context.SaveChangesAsync();

            var command = new RegistrarMovimentoCommand
            {
                RequestId = Guid.NewGuid().ToString(),
                NumeroConta = "12345-6",
                Valor = 100m,
                Tipo = "C"
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            var movimento = await _context.Movimentos.FirstOrDefaultAsync(m => m.RequestId == command.RequestId);
            Assert.NotNull(movimento);
            Assert.Equal(100m, movimento.Valor);
            Assert.Equal("C", movimento.Tipo);
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
            _context.Contas.Add(new Conta
            {
                Id = Guid.NewGuid(),
                Cpf = "12345678901",
                NumeroConta = "12345-6",
                NomeTitular = "Test User",
                Senha = "hash",
                Ativa = true
            });
            await _context.SaveChangesAsync();

            var requestId = Guid.NewGuid().ToString();
            var command = new RegistrarMovimentoCommand
            {
                RequestId = requestId,
                NumeroConta = "12345-6",
                Valor = 100m,
                Tipo = "C"
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            var movimentos = await _context.Movimentos.Where(m => m.RequestId == requestId).ToListAsync();
            Assert.Single(movimentos);
        }

        [Fact]
        public async Task Handle_InactiveAccount_ShouldThrowException()
        {
            // Arrange
            _context.Contas.Add(new Conta
            {
                Id = Guid.NewGuid(),
                Cpf = "12345678901",
                NumeroConta = "12345-6",
                NomeTitular = "Test User",
                Senha = "hash",
                Ativa = false
            });
            await _context.SaveChangesAsync();

            var command = new RegistrarMovimentoCommand
            {
                RequestId = Guid.NewGuid().ToString(),
                NumeroConta = "12345-6",
                Valor = 100m,
                Tipo = "C"
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
