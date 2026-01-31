using MediatR;

namespace BankMore.ContaCorrente.Application.Commands
{
    public class CriarContaComando : IRequest<string>
    {
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public string NomeTitular { get; set; }
    }
}
