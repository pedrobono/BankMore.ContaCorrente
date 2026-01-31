using BankMore.ContaCorrente.Application.DTOs;
using BankMore.ContaCorrente.Application.Queries;
using BankMore.ContaCorrente.Domain.Exceptions;
using BankMore.ContaCorrente.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BankMore.ContaCorrente.Application.Handlers {
    public class ObterSaldoHandler : IRequestHandler<ObterSaldoQuery, SaldoDto> {
        private readonly DataBaseContext _context;

        public ObterSaldoHandler(DataBaseContext context) {
            _context = context;
        }

        public async Task<SaldoDto> Handle(ObterSaldoQuery request, CancellationToken cancellationToken) {
            var conta = await _context.Contas
                .FirstOrDefaultAsync(c => c.Id == request.ContaId, cancellationToken);

            if (conta == null || !conta.Ativa)
                throw new BusinessException("Conta não encontrada ou inativa", "INVALID_ACCOUNT");

            var movimentos = await _context.Movimentos
                .Where(m => m.ContaId == conta.Id)
                .ToListAsync(cancellationToken);

            // Calcula o saldo
            var saldo = movimentos.Where(m => m.Tipo == "C").Sum(m => m.Valor) -
                        movimentos.Where(m => m.Tipo == "D").Sum(m => m.Valor);

            // Se não houver movimentos, o saldo é 0,00
            if (saldo == 0) {
                saldo = 0;
            }

            return new SaldoDto {
                NumeroConta = conta.NumeroConta,
                NomeTitular = conta.NomeTitular,
                Saldo = saldo
            };
        }
    }
}
