# BankMore.ContaCorrente API

A API **BankMore.ContaCorrente** é um serviço de gerenciamento de contas correntes que segue os padrões de **Domain-Driven Design (DDD)**, **CQRS** (Command Query Responsibility Segregation) e **MediatR**. Esta API oferece funcionalidades como criação de contas, autenticação, movimentações de contas e verificação de saldo, com autenticação JWT e validação de dados.

## Tecnologias Utilizadas

- **.NET 8** - Framework principal.
- **MediatR** - Padrão CQRS para separação de comandos e consultas.
- **Swagger** - Documentação interativa da API.
- **JWT Authentication** - Autenticação baseada em tokens JWT.
- **FluentValidation** - Para validação de dados.
- **BCrypt.Net** - Para hash de senhas.
- **SQLite** - Banco de dados relacional (configurável via variável de ambiente).

## Configuração e Instalação

### Passos para rodar localmente:

1. **Clone o repositório**:

   ```bash
   git clone https://github.com/pedrobono/BankMore.ContaCorrente.git
   cd BankMore.ContaCorrente
   ```

2. **Instale as dependências**:

   Para restaurar as dependências do projeto:

   ```bash
   dotnet restore
   ```

3. **Configure as variáveis de ambiente**:

   Defina as variáveis de ambiente para o **JWT_SECRET_KEY** e a string de conexão com o banco de dados.

   **Exemplo (Linux/Mac)**:

   ```bash
   export JWT_SECRET_KEY="SuaChaveSecretaDeProducaoAqui123!"
   export CONNECTIONSTRING="Data Source=BankMore_Production.db"
   ```

   **Exemplo (Windows, PowerShell)**:

   ```powershell
   $env:JWT_SECRET_KEY="SuaChaveSecretaDeProducaoAqui123!"
   $env:CONNECTIONSTRING="Data Source=BankMore_Production.db"
   ```

4. **Execute o projeto**:

   Para rodar a aplicação localmente, execute:

   ```bash
   dotnet run
   ```

   O servidor estará disponível em `https://localhost:5188`.

### Configurações de Banco de Dados

Por padrão, a aplicação usa **SQLite**. A string de conexão é configurada no arquivo `appsettings.json`, mas pode ser substituída por variáveis de ambiente para produção.

### Acessando a API

Acesse a documentação interativa da API via **Swagger**:

- URL: `https://localhost:5188/swagger`

## Endpoints da API

### 1. **Criar Conta**
   **Endpoint:** `POST /contas`

   Cria uma nova conta com os dados fornecidos.

   **Corpo da requisição:**

   ```json
   {
     "cpf": "10010374990",
     "senha": "senha123",
     "nomeTitular": "Pedro Henrique Bono"
   }
   ```

   **Resposta:**

   ```json
   {
     "numeroConta": "53775-2"
   }
   ```

   **Erros:**

   - **400 - CPF Duplicado:**

     ```json
     {
       "message": "O CPF informado já está cadastrado.",
       "failureType": "INVALID_DOCUMENT"
     }
     ```

### 2. **Login (Autenticação)**
   **Endpoint:** `POST /auth/login`

   Realiza o login e retorna um token JWT.

   **Corpo da requisição:**

   ```json
   {
     "cpfOrAccountNumber": "10010374990",
     "senha": "senha123"
   }
   ```

   **Resposta:**

   ```json
   {
     "token": "JWT_Token_Gerado_Aqui"
   }
   ```

### 3. **Inativar Conta**
   **Endpoint:** `PATCH /accounts/me/inactivate`

   Inativa a conta do usuário autenticado.

   **Corpo da requisição:**

   ```json
   {
     "senha": "senha123"
   }
   ```

   **Resposta:**
   - **204 - Sem conteúdo** (caso a conta seja inativada com sucesso).

### 4. **Registrar Movimentos**
   **Endpoint:** `POST /movements`

   Registra um movimento de crédito (C) ou débito (D) na conta do usuário.

   **Corpo da requisição:**

   ```json
   {
     "requestId": "unique-request-id",
     "accountNumber": "53775-2",
     "value": 100.0,
     "type": "C"
   }
   ```

   **Resposta:**
   - **204 - Sem conteúdo** (caso o movimento seja registrado com sucesso).

### 5. **Verificar Saldo**
   **Endpoint:** `GET /balance`

   Recupera o saldo da conta do usuário autenticado.

   **Resposta:**

   ```json
   {
     "accountNumber": "53775-2",
     "holderName": "Pedro Henrique Bono",
     "timestamp": "2026-01-31T02:00:00Z",
     "balance": 100.0
   }
   ```

## Tratamento de Erros

A API retorna erros no formato JSON:

- **Erro de Validação** (por exemplo, CPF duplicado):

  ```json
  {
    "message": "O CPF informado já está cadastrado.",
    "failureType": "INVALID_DOCUMENT"
  }
  ```

- **Erro Inesperado**:

  ```json
  {
    "message": "Ocorreu um erro inesperado.",
    "failureType": "UNKNOWN_ERROR"
  }
  ```

## Testes

### Testes Unitários

- Testes para **Handlers** de comandos e consultas (ex.: criação de conta, validação de CPF).
- Testes para garantir que a lógica de movimentações de contas e cálculos de saldo funcione corretamente.

### Testes de Integração

- **Autenticação**: Testar a geração do token JWT.
- **Movimentos de Conta**: Garantir que a API registre corretamente os movimentos, incluindo validação de idempotência.
- **Saldo**: Testar a consulta de saldo para verificar a soma de créditos e débitos.

## Licença

Este projeto está licenciado sob a licença MIT.

## Como Contribuir

1. Faça o fork do projeto.
2. Crie uma branch para a sua funcionalidade (`git checkout -b feature/minha-feature`).
3. Faça suas alterações e commit (`git commit -am 'Adiciona nova funcionalidade'`).
4. Envie para o seu fork (`git push origin feature/minha-feature`).
5. Abra um pull request.

---

## Sobre o Autor

Este projeto foi desenvolvido por **Pedro Bono**.

- GitHub: [https://github.com/pedrobono](https://github.com/pedrobono)
- LinkedIn: [https://www.linkedin.com/in/pedro-h-bono/](https://www.linkedin.com/in/pedro-h-bono/)

Se você tiver dúvidas, sugestões ou quiser colaborar no projeto, fique à vontade para abrir um **issue** ou enviar um **pull request**.
