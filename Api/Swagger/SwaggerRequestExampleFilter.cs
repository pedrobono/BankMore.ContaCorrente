using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;

namespace BankMore.ContaCorrente.Api.Swagger
{
    public class SwaggerRequestExampleFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Verifica se o corpo da requisição é JSON
            if (operation.RequestBody != null && operation.RequestBody.Content.ContainsKey("application/json"))
            {
                var schema = operation.RequestBody.Content["application/json"].Schema;

                // Verifica se o método sendo processado é o "CriarConta"
                if (context.MethodInfo.Name == "CriarConta")
                {
                    // Definindo exemplos para os campos cpf, senha e nomeTitular
                    schema.Properties["cpf"].Example = new OpenApiString("10010374990");
                    schema.Properties["senha"].Example = new OpenApiString("senha123");
                    schema.Properties["nomeTitular"].Example = new OpenApiString("Pedro Henrique Bono");
                }
            }
        }
    }
}
