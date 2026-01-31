namespace BankMore.ContaCorrente.Domain.Entities {
    public class Movimento {
        public Guid Id { get; set; }
        public Guid ContaId { get; set; }
        public Conta Conta { get; set; }
        public string Tipo { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataHora { get; set; }
        public string RequestId { get; set; }
    }
}
