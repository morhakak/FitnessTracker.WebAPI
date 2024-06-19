using System.Text;

namespace FitnessTracker.WebAPI.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private readonly ILogger<RequestLoggingMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        _logger.LogInformation($"Incoming request: {context.Request.Method} {context.Request.Path}");

        if (context.Request.ContentLength > 0)
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true);
            var requestBody = await reader.ReadToEndAsync();
            _logger.LogInformation($"Request body: {requestBody}");
            context.Request.Body.Position = 0;
        }

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while processing the request: {ex.Message}");
            throw; 
        }
    }
}
