using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using System.Linq;
using Microsoft.OpenApi.Any;

namespace BankMore.ContaCorrente.Api.Swagger
{
    public class SwaggerResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Adiciona a resposta 400 (erro de CPF duplicado)
            if (operation.Responses.ContainsKey("400"))
            {
                operation.Responses["400"] = new OpenApiResponse
                {
                    Description = "Erro de validação ou CPF duplicado",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            "application/json", new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "object",
                                    Properties = new Dictionary<string, OpenApiSchema>
                                    {
                                        { "message", new OpenApiSchema { Type = "string", Example = new OpenApiString("O CPF informado já está cadastrado.") } },
                                        { "failureType", new OpenApiSchema { Type = "string", Example = new OpenApiString("INVALID_DOCUMENT") } }
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}
