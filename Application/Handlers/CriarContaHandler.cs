using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Domain.Exceptions;

namespace BankMore.ContaCorrente.Application.Handlers {
    public class CriarContaHandler : IRequestHandler<CriarContaCommand, string> {
        private readonly DataBaseContext _contexto;

        public CriarContaHandler(DataBaseContext contexto) {
            _contexto = contexto;
        }

        public async Task<string> Handle(CriarContaCommand request, CancellationToken cancellationToken) {
            var cpfExistente = await _contexto.Contas
                .AnyAsync(c => c.Cpf == request.Cpf, cancellationToken);

            if (cpfExistente) {
                throw new BusinessException("O CPF informado já está cadastrado.", "DUPLICATE_CPF");
            }

            var cpf = new Domain.ValueObjects.Cpf(request.Cpf);

            string numeroGerado;
            bool numeroJaExiste;

            do {
                numeroGerado = GerarNumeroUnico();
                numeroJaExiste = await _contexto.Contas.AnyAsync(c => c.NumeroConta == numeroGerado, cancellationToken);
            } while (numeroJaExiste);

            var conta = new Conta {
                Id = Guid.NewGuid(),
                NumeroConta = numeroGerado,
                NomeTitular = request.NomeTitular,
                Senha = BCrypt.Net.BCrypt.HashPassword(request.Senha),
                Ativa = true,
                Cpf = cpf.Valor
            };

            _contexto.Contas.Add(conta);
            await _contexto.SaveChangesAsync(cancellationToken);

            return conta.NumeroConta;
        }

        private string GerarNumeroUnico() {
            var random = new Random();
            return $"{random.Next(10000, 99999)}-{random.Next(0, 9)}";
        }
    }
}
