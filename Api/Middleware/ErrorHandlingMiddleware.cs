using BankMore.ContaCorrente.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BankMore.ContaCorrente.Api.Middleware {
    public class ErrorHandlingMiddleware {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger) {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext) {
            try {
                await _next(httpContext);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Erro inesperado ocorrido durante o processamento da requisição.");

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception) {
            var statusCode = exception switch {
                BusinessException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new {
                message = exception.Message,
                failureType = exception is BusinessException ? ((BusinessException)exception).FailureType : "INTERNAL_ERROR"
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
