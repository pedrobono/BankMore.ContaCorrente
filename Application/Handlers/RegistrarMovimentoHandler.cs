using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Infrastructure.Data;
using BankMore.ContaCorrente.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Domain.Exceptions;

namespace BankMore.ContaCorrente.Application.Handlers {
    public class RegistrarMovimentoHandler : IRequestHandler<RegistrarMovimentoCommand> {
        private readonly DataBaseContext _context;

        public RegistrarMovimentoHandler(DataBaseContext context) {
            _context = context;
        }

        public async Task Handle(RegistrarMovimentoCommand request, CancellationToken cancellationToken) {
            var conta = await _context.Contas
                .FirstOrDefaultAsync(c => c.NumeroConta == request.NumeroConta, cancellationToken);

            if (conta == null)
                throw new BusinessException("Conta não encontrada", "INVALID_ACCOUNT");

            if (!conta.Ativa)
                throw new BusinessException("Esta conta está encerrada ou inativa", "INACTIVE_ACCOUNT");

            if (request.Valor <= 0)
                throw new BusinessException("O valor da movimentação deve ser positivo", "INVALID_VALUE");

            if (request.Tipo != "C" && request.Tipo != "D")
                throw new BusinessException("Tipo de movimento inválido", "INVALID_TYPE");

            // Débito só pode ser feito na própria conta (quando ContaIdLogada está presente)
            if (request.Tipo == "D" && request.ContaIdLogada.HasValue && conta.Id != request.ContaIdLogada.Value)
                throw new BusinessException("Débito só pode ser realizado na própria conta", "INVALID_TYPE");

            var movimentoExistente = await _context.Movimentos
                .AnyAsync(m => m.ContaId == conta.Id && m.RequestId == request.RequestId, cancellationToken);

            if (movimentoExistente)
                return;

            // Validar saldo insuficiente para débitos
            if (request.Tipo == "D") {
                var saldoAtual = await _context.Movimentos
                    .Where(m => m.ContaId == conta.Id)
                    .SumAsync(m => m.Tipo == "C" ? m.Valor : -m.Valor, cancellationToken);

                if (saldoAtual < request.Valor)
                    throw new BusinessException("Saldo insuficiente", "INSUFFICIENT_BALANCE");
            }

            var movimento = new Movimento {
                ContaId = conta.Id,
                Tipo = request.Tipo,
                Valor = request.Valor,
                DataHora = DateTime.UtcNow,
                RequestId = request.RequestId
            };

            _context.Movimentos.Add(movimento);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
