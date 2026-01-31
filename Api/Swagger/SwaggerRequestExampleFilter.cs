using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using BankMore.ContaCorrente.Application.Commands;

namespace BankMore.ContaCorrente.Api.Swagger;

public class SwaggerRequestExampleFilter : IOperationFilter {
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        if (context.MethodInfo.Name == "CriarConta") {
            operation.RequestBody.Content["application/json"].Example = new OpenApiObject {
                ["cpf"] = new OpenApiString("10010374990"),
                ["senha"] = new OpenApiString("senha123"),
                ["nomeTitular"] = new OpenApiString("Pedro Henrique Bono")
            };
        }

        if (context.MethodInfo.Name == "RegistrarMovimento") {
            operation.RequestBody.Content["application/json"].Example = new OpenApiObject {
                ["accountNumber"] = new OpenApiString("12345-6"),
                ["value"] = new OpenApiDouble(150.50),
                ["type"] = new OpenApiString("C"), // C para Crédito, D para Débito
                ["requestId"] = new OpenApiString(Guid.NewGuid().ToString())
            };
        }
    }
}