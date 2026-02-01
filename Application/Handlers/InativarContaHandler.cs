using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Domain.Exceptions;
using BankMore.ContaCorrente.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BankMore.ContaCorrente.Application.Handlers {
    public class InativarContaHandler : IRequestHandler<InativarContaCommand> {
        private readonly DataBaseContext _context;

        public InativarContaHandler(DataBaseContext context) {
            _context = context;
        }

        public async Task Handle(InativarContaCommand request, CancellationToken cancellationToken) {
            var conta = await _context.Contas
                .FirstOrDefaultAsync(c => c.Id == request.ContaId, cancellationToken);

            if (conta == null)
                throw new BusinessException("Conta não encontrada", "INVALID_ACCOUNT");

            if (!BCrypt.Net.BCrypt.Verify(request.Senha, conta.Senha))
                throw new UnauthorizedAccessException("Senha inválida");

            conta.Ativa = false;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
