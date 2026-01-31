using BankMore.ContaCorrente.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BankMore.ContaCorrente.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Continua o pipeline de requisição normalmente
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Logar qualquer exceção que ocorrer
                _logger.LogError(ex, "Erro inesperado ocorrido durante o processamento da requisição.");

                // Retorna uma resposta de erro de forma genérica
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Definir o código de status conforme o tipo de exceção
            var statusCode = exception switch
            {
                BusinessException => StatusCodes.Status400BadRequest, // Erro de lógica de negócio (ex: CPF duplicado)
                _ => StatusCodes.Status500InternalServerError // Qualquer outro erro inesperado
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                message = exception.Message, // Mensagem do erro
                failureType = exception is BusinessException ? "INVALID_DOCUMENT" : "UNKNOWN_ERROR"
            };

            // Retorna o erro em formato JSON
            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
