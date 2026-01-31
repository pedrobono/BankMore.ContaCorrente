using BankMore.ContaCorrente.Application.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BankMore.ContaCorrente.Application.Handlers
{
    public class CriarContaManipulador : IRequestHandler<CriarContaComando, string>
    {
        public async Task<string> Handle(CriarContaComando request, CancellationToken cancellationToken)
        {
            // Lógica para criar a conta e retornar o número
            var numeroConta = Guid.NewGuid().ToString().Substring(0, 6); // Exemplo simplificado
            return numeroConta;
        }
    }
}
