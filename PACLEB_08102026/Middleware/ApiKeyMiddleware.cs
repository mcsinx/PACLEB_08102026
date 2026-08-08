using System.Security.Cryptography;
using System.Text;

namespace PACLEB_08102026.Middleware;

public sealed class ApiKeyMiddleware
{
    private const string ApiKeyHeaderName = "X-API-Key";

    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiKeyMiddleware> _logger;

    public ApiKeyMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api/files"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(
                ApiKeyHeaderName,
                out var providedApiKey))
        {
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            await context.Response.WriteAsJsonAsync(new
            {
                error = "API key is required."
            });

            return;
        }

        var configuredApiKey =
            _configuration["ApiKey"];

        if (string.IsNullOrWhiteSpace(configuredApiKey))
        {
            _logger.LogError("API key is not configured.");

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            return;
        }

        if (!IsValidApiKey(
                providedApiKey.ToString(),
                configuredApiKey))
        {
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            await context.Response.WriteAsJsonAsync(new
            {
                error = "Invalid API key."
            });

            return;
        }

        await _next(context);
    }

    private static bool IsValidApiKey(
        string provided,
        string expected)
    {
        var providedBytes = Encoding.UTF8.GetBytes(provided);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);

        return providedBytes.Length == expectedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(
                   providedBytes,
                   expectedBytes);
    }
}