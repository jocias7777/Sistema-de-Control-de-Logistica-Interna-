using DespachoLogistica.API.Models.Common;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Text.Json;

namespace DespachoLogistica.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            ApiResponse<object> response;

            if (ex is SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    response = ApiResponse<object>.Fail("Ya existe un registro con esos datos.", 409);
                }
                else if (sqlEx.Number == 547)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    response = ApiResponse<object>.Fail("No se puede eliminar, tiene registros asociados.", 409);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = ApiResponse<object>.Fail("Error interno del servidor.", 500);
                }
            }
            else if (ex is ArgumentException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = ApiResponse<object>.Fail(ex.Message, 400);
            }
            else if (ex is UnauthorizedAccessException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                response = ApiResponse<object>.Fail("No tiene permisos para esta acción.", 403);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = ApiResponse<object>.Fail("Error interno del servidor.", 500);
            }

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}