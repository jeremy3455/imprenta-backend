using System.Net;
using System.Text.Json;

namespace ImprentaSR.WebAPI.Middleware;

/// <summary>
/// Middleware global para el manejo centralizado de excepciones.
/// Captura excepciones no controladas y devuelve respuestas JSON
/// con el código HTTP adecuado y un mensaje descriptivo.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    /// <summary>
    /// Constructor que recibe el siguiente middleware en el pipeline.
    /// </summary>
    /// <param name="next">Siguiente delegado del pipeline.</param>
    /// <param name="logger">Logger para registrar las excepciones.</param>
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Ejecuta el middleware. Si ocurre una excepción, la captura y
    /// devuelve una respuesta JSON estructurada.
    /// </summary>
    /// <param name="context">Contexto HTTP actual.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                statusCode = 404,
                message = ex.Message
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                statusCode = 500,
                message = "An internal server error occurred."
            }));
        }
    }
}
