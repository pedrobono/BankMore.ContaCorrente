using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;
using BankMore.ContaCorrente.Application.Handlers;
using BankMore.ContaCorrente.Application.Queries;
using BankMore.ContaCorrente.Infrastructure.Data;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BankMore.ContaCorrente.Tests.UnitTests
{
    public class ObterSaldoHandlerTests
    {
        private readonly DataBaseContext _context;
        private readonly ObterSaldoHandler _handler;

        public ObterSaldoHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new DataBaseContext(options);
            _handler = new ObterSaldoHandler(_context);
        }

        [Fact]
        public async Task Handle_ValidAccountId_ReturnsCorrectBalance()
        {
            // Arrange
            var contaId = Guid.NewGuid();
            _context.Contas.Add(new Conta
            {
                Id = contaId,
                Cpf = "12345678901",
                NumeroConta = "12345-6",
                NomeTitular = "Test User",
                Senha = "hash",
                Ativa = true
            });
            _context.Movimentos.Add(new Movimento { Id = Guid.NewGuid(), ContaId = contaId, Tipo = "C", Valor = 100m, DataHora = DateTime.Now, RequestId = Guid.NewGuid().ToString() });
            _context.Movimentos.Add(new Movimento { Id = Guid.NewGuid(), ContaId = contaId, Tipo = "D", Valor = 30m, DataHora = DateTime.Now, RequestId = Guid.NewGuid().ToString() });
            await _context.SaveChangesAsync();

            var query = new ObterSaldoQuery { ContaId = contaId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(70m, result.Saldo);
            Assert.Equal("12345-6", result.NumeroConta);
            Assert.Equal("Test User", result.NomeTitular);
        }

        [Fact]
        public async Task Handle_InvalidAccountId_ThrowsException()
        {
            // Arrange
            var query = new ObterSaldoQuery { ContaId = Guid.NewGuid() };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }
    }
}
