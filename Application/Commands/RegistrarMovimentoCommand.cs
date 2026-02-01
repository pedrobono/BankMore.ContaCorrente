using MediatR;

namespace BankMore.ContaCorrente.Application.Commands {
    public class RegistrarMovimentoCommand : IRequest {
        public string RequestId { get; set; }
        public string? NumeroConta { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; }
        public Guid? ContaIdLogada { get; set; }
    }
}
