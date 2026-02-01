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

            if (conta == null)
                throw new BusinessException("Conta não encontrada", "INVALID_ACCOUNT");

            if (!conta.Ativa)
                throw new BusinessException("Conta inativa", "INACTIVE_ACCOUNT");

            var movimentos = await _context.Movimentos
                .Where(m => m.ContaId == conta.Id)
                .ToListAsync(cancellationToken);

            var saldo = movimentos.Where(m => m.Tipo == "C").Sum(m => m.Valor) -
                        movimentos.Where(m => m.Tipo == "D").Sum(m => m.Valor);

            return new SaldoDto {
                NumeroConta = conta.NumeroConta,
                NomeTitular = conta.NomeTitular,
                DataHoraConsulta = DateTime.UtcNow,
                Saldo = saldo
            };
        }
    }
}
