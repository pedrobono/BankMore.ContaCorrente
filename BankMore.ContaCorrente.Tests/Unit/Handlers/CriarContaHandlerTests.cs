using Xunit;
using System.Threading;
using System;
using System.Threading.Tasks;
using BankMore.ContaCorrente.Application.Handlers;
using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Infrastructure.Data;
using BankMore.ContaCorrente.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Domain.Exceptions;

namespace BankMore.ContaCorrente.Tests.UnitTests
{
    public class CriarContaHandlerTests
    {
        private readonly DataBaseContext _context;
        private readonly CriarContaHandler _handler;

        public CriarContaHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new DataBaseContext(options);
            _handler = new CriarContaHandler(_context);
        }

        [Fact]
        public async Task Handle_DadosValidos_DeveRetornarNumeroDaContaGerado()
        {
            // Arrange
            var command = new CriarContaCommand 
            { 
                Cpf = "52998224725", 
                NomeTitular = "Pedro Bono", 
                Senha = "senha123" 
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(result));
            Assert.Contains("-", result);
            
            var contaSalva = await _context.Contas.FirstOrDefaultAsync(c => c.NumeroConta == result);
            Assert.NotNull(contaSalva);
            Assert.Equal("Pedro Bono", contaSalva.NomeTitular);
        }

        [Fact]
        public async Task Handle_CpfJaExistente_DeveLancarExcecao()
        {
            // Arrange
            _context.Contas.Add(new Conta 
            { 
                Id = Guid.NewGuid(), 
                Cpf = "11122233344", 
                NumeroConta = "12345-6", 
                NomeTitular = "Existente", 
                Senha = "hash", 
                Ativa = true 
            });
            await _context.SaveChangesAsync();

            var command = new CriarContaCommand { Cpf = "11122233344", NomeTitular = "Repetido", Senha = "123" };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => 
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
