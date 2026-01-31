# BankMore.ContaCorrente.Tests

This project contains integration tests for the controllers and unit tests for the handlers of the BankMore.ContaCorrente application.

## Project Structure

- **Integration**
  - **Controllers**
    - `AuthControllerTests.cs`: Integration tests for the AuthController, verifying authentication endpoints.
    - `ContaControllerTests.cs`: Integration tests for the ContaController, ensuring account-related endpoints function correctly.
    - `MovimentoControllerTests.cs`: Integration tests for the MovimentoController, testing transaction processing.
    - `SaldoControllerTests.cs`: Integration tests for the SaldoController, checking balance retrieval accuracy.

- **Unit**
  - **Handlers**
    - `CriarContaHandlerTests.cs`: Unit tests for the CriarContaHandler, validating account creation logic.
    - `LoginHandlerTests.cs`: Unit tests for the LoginHandler, verifying login logic for various scenarios.
    - `ObterSaldoHandlerTests.cs`: Unit tests for the ObterSaldoHandler, ensuring correct balance retrieval.
    - `RegistrarMovimentoHandlerTests.cs`: Unit tests for the RegistrarMovimentoHandler, validating transaction registration logic.

## Running Tests

To run the tests in this project, you can use the following command in your terminal:

```
dotnet test
```

Ensure that you have the .NET SDK installed and that you are in the project directory.

## Additional Information

For more details on specific tests or to add new tests, please refer to the individual test files in the respective directories.