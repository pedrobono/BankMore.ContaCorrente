using BankMore.ContaCorrente.Application.Commands;
using FluentValidation;

namespace BankMore.ContaCorrente.Application.Validators
{
    public class CriarContaValidador : AbstractValidator<CriarContaCommand>
    {
        public CriarContaValidador()
        {
            RuleFor(x => x.Cpf).NotEmpty().Length(11).WithMessage("CPF inválido");
            RuleFor(x => x.Senha).NotEmpty().MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres");
            RuleFor(x => x.NomeTitular).NotEmpty().WithMessage("Nome do titular é obrigatório");
        }
    }
}
