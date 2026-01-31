using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Domain.Interfaces {
    public interface IContaRepository {
        Task<Conta?> ObterPorNumero(string numeroConta);
        Task<Conta?> ObterPorCpf(string cpf);
        Task Adicionar(Conta conta);
        Task Atualizar(Conta conta);
    }
}