namespace BankMore.ContaCorrente.Domain.Entities {
    public class Transferencia {
        public Guid Id { get; set; }
        public Guid ContaOrigemId { get; set; }
        public Conta ContaOrigem { get; set; }
        public Guid ContaDestinoId { get; set; }
        public Conta ContaDestino { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataHora { get; set; }
        public string RequestId { get; set; }
    }
}
