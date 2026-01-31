# BankMore.ContaCorrente API

A API **BankMore.ContaCorrente** é um serviço de gerenciamento bancário de alta performance desenvolvido em **.NET 8**. O projeto utiliza **Domain-Driven Design (DDD)** e o padrão **CQRS** com **MediatR** para garantir um processamento de transações escalável, seguro e resiliente.

## 🚀 Tecnologias e Padrões
- **.NET 8** - Core da aplicação.
- **CQRS & MediatR** - Separação clara entre comandos de escrita e consultas.
- **JWT Authentication** - Segurança via tokens Bearer com suporte a esquemas de autorização.
- **FluentValidation** - Garantia de integridade dos dados de entrada.
- **SQLite** - Persistência relacional com suporte a Migrations.
- **Swagger/OpenAPI 3.0** - Documentação técnica automatizada e padronizada.

## 🛠️ Instalação e Execução

1. **Clonagem e Dependências**:
   bash
   git clone [https://github.com/pedrobono/BankMore.ContaCorrente.git](https://github.com/pedrobono/BankMore.ContaCorrente.git)
   cd BankMore.ContaCorrente
   dotnet restore


2. **Ambiente (Ubuntu/Linux)**:
bash
export JWT_SECRET_KEY="SuaChaveSecretaDeProducaoAqui123!"
export CONNECTIONSTRING="Data Source=BankMore.db"




3. **Banco de Dados e Execução**:
bash
dotnet ef database update
dotnet run




Acesse: `http://localhost:5188/swagger`

## 📍 Endpoints da API

### 🔐 Autenticação (`/api/Auth/login`)

* **POST**: Autentica via CPF ou Conta. Retorna um `LoginResponse` contendo o Token JWT.

### 🏦 Gerenciamento de Conta (`/api/Conta`)

* **POST**: Criação de conta corrente.
* **Exemplo de Retorno**: `{ "numeroConta": "85381-6" }`



### 💸 Movimentações (`/api/Movimento`)

* **POST**: Registra Crédito (`C`) ou Débito (`D`).
* **Idempotência**: Exige um `requestId` (UUID) para evitar duplicidade de transações.
* **Segurança**: Requer cabeçalho `Authorization: Bearer <token>`.

### 💰 Consultas (`/api/Saldo`)

* **GET**: Retorna o `SaldoDto` contendo o número da conta, nome do titular e saldo atualizado.

## 🛡️ Tratamento de Erros Padronizado

Todas as respostas de falha seguem o padrão definido para facilitar a integração com front-ends:

* **400 (Bad Request)**: Erros de validação ou regras de negócio (ex: Saldo Insuficiente).
* **401 (Unauthorized)**: Token ausente, expirado ou credenciais inválidas.

Exemplo de erro:

json
{
  "message": "Descrição amigável do erro",
  "failureType": "INVALID_DATA"
}



## 🤝 Contribuição

1. Fork o projeto.
2. Crie sua Feature Branch (`git checkout -b feature/NovaFeature`).
3. Commit suas mudanças (`git commit -m 'feat: Descrição da feature'`).
4. Push para a Branch (`git push origin feature/NovaFeature`).
5. Abra um Pull Request.

## ⚖️ Licença

Este projeto está sob a licença **MIT**. Veja o arquivo [LICENSE](https://www.google.com/search?q=LICENSE) para detalhes.

---

## 👨‍💻 Autor

**Pedro Bono**

* [GitHub](https://github.com/pedrobono)
* [LinkedIn](https://www.linkedin.com/in/pedro-h-bono/)