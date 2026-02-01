# BankMore.ContaCorrente API

A API **BankMore.ContaCorrente** é um serviço de gerenciamento bancário de alta performance desenvolvido em **.NET 8**. O projeto utiliza **Domain-Driven Design (DDD)** e o padrão **CQRS** com **MediatR** para garantir um processamento de transações escalável, seguro e resiliente.

## 🚀 Tecnologias e Padrões
- **.NET 8** - Core da aplicação
- **DDD (Domain-Driven Design)** - Organização em camadas (Domain, Application, Infrastructure, API)
- **CQRS & MediatR** - Separação clara entre comandos de escrita e consultas
- **JWT Authentication** - Segurança via tokens Bearer
- **FluentValidation** - Validação de entrada de dados
- **Entity Framework Core** - ORM com suporte a migrations
- **SQLite** - Persistência relacional (pronto para Postgres/SQL Server)
- **Swagger/OpenAPI 3.0** - Documentação interativa da API
- **xUnit** - Framework de testes unitários e de integração
- **Docker** - Containerização multi-stage para produção

## ✨ Funcionalidades

### Segurança
- 🔐 Autenticação JWT com expiração
- 🔒 Autorização por token em endpoints protegidos
- 🔑 Hash de senhas com BCrypt
- ✅ Validação de CPF com algoritmo oficial

### Transações
- 🔁 **Idempotência**: Evita duplicação de transações via `requestId`
- 📊 **Validação de Saldo**: Impede débitos com saldo insuficiente
- 💳 Créditos e débitos com validações de negócio
- 📄 Consulta de saldo em tempo real

### Arquitetura
- 🏛️ DDD com separação de responsabilidades
- 🔄 CQRS para escalabilidade
- 📦 Pronto para microsserviços (Transfer Service pode consumir)
- 🐳 Docker multi-stage para builds otimizados

## 🛠️ Instalação e Execução

### Execução Local

1. **Clonagem e Dependências**:
   ```bash
   git clone https://github.com/pedrobono/BankMore.ContaCorrente.git
   cd BankMore.ContaCorrente
   dotnet restore
   ```

2. **Ambiente (Ubuntu/Linux)**:
   ```bash
   export JWT_SECRET_KEY="SuaChaveSecretaDeProducaoAqui123!"
   export CONNECTIONSTRING="Data Source=BankMore.db"
   ```

3. **Banco de Dados e Execução**:
   ```bash
   dotnet ef database update
   dotnet run
   ```

Acesse: `http://localhost:5188/swagger`

### Execução com Docker

1. **Build da imagem**:
   ```bash
   docker build -t bankmore-account-service:latest .
   ```

2. **Executar container**:
   ```bash
   docker run -d -p 8081:8081 \
     -e JWT_SECRET_KEY="SuaChaveSecretaDeProducaoAqui123!" \
     -e ConnectionStrings__DefaultConnection="Data Source=/app/data/BankMore.db" \
     -v $(pwd)/data:/app/data \
     --name account-service \
     bankmore-account-service:latest
   ```

Acesse: `http://localhost:8081/swagger`

## 📍 Endpoints da API

### 🔐 Autenticação (`/api/Auth`)

#### POST `/api/Auth/login`
Autentica usuário via CPF ou número da conta.

**Request Body:**
```json
{
  "cpfOrNumeroConta": "12345678909",
  "senha": "senha123"
}
```

**Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Erros:**
- `401 UNAUTHORIZED`: Credenciais inválidas (failureType: `USER_UNAUTHORIZED`)

---

### 🏦 Gerenciamento de Conta (`/api/Conta`)

#### POST `/api/Conta`
Cria uma nova conta corrente.

**Request Body:**
```json
{
  "cpf": "12345678909",
  "nomeTitular": "João Silva",
  "senha": "senha123"
}
```

**Response (201):**
```json
{
  "numeroConta": "85381-6"
}
```

**Erros:**
- `400 BAD REQUEST`: CPF inválido (failureType: `INVALID_DOCUMENT`)

#### POST `/api/Conta/resolve` 🔒
Resolve número da conta para ID interno. Usado pelo Transfer Service.

**Headers:** `Authorization: Bearer <token>`

**Request Body:**
```json
{
  "numeroConta": "85381-6"
}
```

**Response (200):**
```json
{
  "contaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "numeroConta": "85381-6"
}
```

**Erros:**
- `403 FORBIDDEN`: Token inválido ou expirado
- `404 NOT FOUND`: Conta não encontrada (failureType: `INVALID_ACCOUNT`)

#### PATCH `/api/Conta/me/inactivate` 🔒
Inativa a conta do usuário autenticado.

**Headers:** `Authorization: Bearer <token>`

**Request Body:**
```json
{
  "senha": "senha123"
}
```

**Response (204):** No Content

**Erros:**
- `401 UNAUTHORIZED`: Senha incorreta
- `403 FORBIDDEN`: Token inválido ou expirado

---

### 💸 Movimentações (`/api/Movimento`)

#### POST `/api/Movimento` 🔒
Registra crédito ou débito na conta.

**Headers:** `Authorization: Bearer <token>`

**Request Body:**
```json
{
  "requestId": "550e8400-e29b-41d4-a716-446655440000",
  "numeroConta": "85381-6",
  "valor": 100.50,
  "tipo": "C"
}
```

**Campos:**
- `requestId`: UUID para idempotência (obrigatório)
- `numeroConta`: Número da conta (opcional, usa conta do token se omitido)
- `valor`: Valor da movimentação (deve ser > 0)
- `tipo`: `"C"` (Crédito) ou `"D"` (Débito)

**Response (204):** No Content

**Validações:**
- ✅ Conta deve existir e estar ativa
- ✅ Valor deve ser positivo
- ✅ Débito só pode ser feito na própria conta
- ✅ Crédito pode ser feito em qualquer conta
- ✅ Débito requer saldo suficiente
- ✅ Idempotente: mesmo `requestId` não duplica operação

**Erros:**
- `400 BAD REQUEST`:
  - `INVALID_ACCOUNT`: Conta não encontrada
  - `INACTIVE_ACCOUNT`: Conta inativa
  - `INVALID_VALUE`: Valor inválido (≤ 0)
  - `INVALID_TYPE`: Tipo inválido ou débito em conta de terceiro
  - `INSUFFICIENT_BALANCE`: Saldo insuficiente para débito
- `403 FORBIDDEN`: Token inválido ou expirado

---

### 💰 Consultas (`/api/Saldo`)

#### GET `/api/Saldo` 🔒
Retorna o saldo atual da conta autenticada.

**Headers:** `Authorization: Bearer <token>`

**Response (200):**
```json
{
  "numeroConta": "85381-6",
  "nomeTitular": "João Silva",
  "dataHora": "2024-01-31T10:30:00Z",
  "saldo": 1500.75
}
```

**Cálculo:** Saldo = Σ Créditos - Σ Débitos

**Erros:**
- `400 BAD REQUEST`:
  - `INVALID_ACCOUNT`: Conta não encontrada
  - `INACTIVE_ACCOUNT`: Conta inativa
- `403 FORBIDDEN`: Token inválido ou expirado

## 🛡️ Tratamento de Erros Padronizado

Todas as respostas de falha seguem o padrão RFC 7807 para facilitar a integração:

### Códigos HTTP
- **400 (Bad Request)**: Erros de validação ou regras de negócio
- **401 (Unauthorized)**: Credenciais inválidas
- **403 (Forbidden)**: Token ausente, inválido ou expirado
- **404 (Not Found)**: Recurso não encontrado

### Formato de Erro
```json
{
  "message": "Descrição amigável do erro",
  "failureType": "TIPO_DO_ERRO"
}
```

### Tipos de Falha (failureType)
| Tipo | Descrição | HTTP |
|------|-------------|------|
| `INVALID_DOCUMENT` | CPF inválido | 400 |
| `USER_UNAUTHORIZED` | Credenciais incorretas | 401 |
| `INVALID_TOKEN` | Token inválido/expirado | 403 |
| `INVALID_ACCOUNT` | Conta não encontrada | 400/404 |
| `INACTIVE_ACCOUNT` | Conta inativa | 400 |
| `INVALID_VALUE` | Valor inválido (≤ 0) | 400 |
| `INVALID_TYPE` | Tipo de movimento inválido | 400 |
| `INSUFFICIENT_BALANCE` | Saldo insuficiente | 400 |
| `INVALID_DATA` | Dados de entrada inválidos | 400 |

---

## 🧪 Testes

O projeto possui cobertura completa de testes:

### Executar todos os testes
```bash
dotnet test
```

### Executar testes específicos
```bash
# Testes de integração
dotnet test --filter "FullyQualifiedName~Integration"

# Testes unitários
dotnet test --filter "FullyQualifiedName~Unit"

# Testes de saldo insuficiente
dotnet test --filter "FullyQualifiedName~SaldoInsuficienteTests"
```

### Cobertura
- ✅ **65 testes** (100% passando)
- ✅ Testes unitários de handlers
- ✅ Testes de integração de controllers
- ✅ Testes de idempotência
- ✅ Testes de validação de saldo
- ✅ Testes de autenticação e autorização



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