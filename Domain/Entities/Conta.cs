namespace BankMore.ContaCorrente.Domain.Entities;

public class Conta
{
        public Guid IdConta { get; set; }
        public string NumeroConta { get; set; }
        public string NomeTitular { get; set; }
        public string Senha { get; set; }
        public bool Ativa { get; set; }
        public string Cpf { get; set; }
    
}
