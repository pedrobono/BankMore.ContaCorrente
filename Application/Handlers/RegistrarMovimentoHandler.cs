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
            Conta conta;
            
            if (string.IsNullOrEmpty(request.NumeroConta)) {
                if (!request.ContaIdLogada.HasValue)
                    throw new BusinessException("Conta não identificada", "INVALID_ACCOUNT");
                    
                conta = await _context.ContaCorrente
                    .FirstOrDefaultAsync(c => c.IdContaCorrente == request.ContaIdLogada.Value, cancellationToken);
            } else {
                var numeroFormatado = request.NumeroConta.Replace("-", "");
                var todasContas = await _context.ContaCorrente.ToListAsync(cancellationToken);
                conta = todasContas.FirstOrDefault(c => $"{c.Numero}{c.Numero % 10}" == numeroFormatado);
            }

            if (conta == null)
                throw new BusinessException("Conta não encontrada", "INVALID_ACCOUNT");

            if (conta.Ativo == 0)
                throw new BusinessException("Esta conta está encerrada ou inativa", "INACTIVE_ACCOUNT");

            if (request.Valor <= 0)
                throw new BusinessException("O valor da movimentação deve ser positivo", "INVALID_VALUE");

            if (request.Tipo != "C" && request.Tipo != "D")
                throw new BusinessException("Tipo de movimento inválido", "INVALID_TYPE");

            if (request.Tipo == "D" && request.ContaIdLogada.HasValue && conta.IdContaCorrente != request.ContaIdLogada.Value)
                throw new BusinessException("Débito só pode ser realizado na própria conta", "INVALID_TYPE");

            var movimentoExistente = await _context.Idempotencia
                .AnyAsync(i => i.ChaveIdempotencia == Guid.Parse(request.RequestId), cancellationToken);

            if (movimentoExistente)
                return;

            if (request.Tipo == "D") {
                var movimentos = await _context.Movimento
                    .Where(m => m.IdContaCorrente == conta.IdContaCorrente)
                    .ToListAsync(cancellationToken);
                    
                var saldoAtual = movimentos.Sum(m => m.TipoMovimento == "C" ? m.Valor : -m.Valor);

                if (saldoAtual < request.Valor)
                    throw new BusinessException("Saldo insuficiente", "INSUFFICIENT_BALANCE");
            }

            var movimento = new Movimento {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = conta.IdContaCorrente,
                TipoMovimento = request.Tipo,
                Valor = request.Valor,
                DataMovimento = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")
            };

            var idempotencia = new Idempotencia {
                ChaveIdempotencia = Guid.Parse(request.RequestId),
                Requisicao = $"{{\"tipo\":\"{request.Tipo}\",\"valor\":{request.Valor}}}",
                Resultado = "SUCCESS"
            };

            _context.Movimento.Add(movimento);
            _context.Idempotencia.Add(idempotencia);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
