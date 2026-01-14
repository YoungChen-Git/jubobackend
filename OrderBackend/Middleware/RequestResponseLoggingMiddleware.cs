using System.Diagnostics;
using System.Text;

namespace OrderBackend.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 記錄 Request
        await LogRequest(context);

        // 保存原始的 Response Body Stream
        var originalBodyStream = context.Response.Body;

        try
        {
            // 使用 MemoryStream 來捕獲 Response
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // 記錄開始時間
            var stopwatch = Stopwatch.StartNew();

            // 執行下一個 Middleware
            await _next(context);

            // 停止計時
            stopwatch.Stop();

            // 記錄 Response
            await LogResponse(context, stopwatch.ElapsedMilliseconds);

            // 將 Response 寫回原始的 Stream
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogRequest(HttpContext context)
    {
        context.Request.EnableBuffering();

        var request = context.Request;
        var requestBody = string.Empty;

        if (request.ContentLength > 0)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(
                request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
        }

        _logger.LogInformation(
            "HTTP Request: {Method} {Path} {QueryString}\n" +
            "Headers: {Headers}\n" +
            "Body: {Body}",
            request.Method,
            request.Path,
            request.QueryString,
            FormatHeaders(request.Headers),
            requestBody);
    }

    private async Task LogResponse(HttpContext context, long elapsedMilliseconds)
    {
        var response = context.Response;
        response.Body.Seek(0, SeekOrigin.Begin);

        var responseBody = string.Empty;
        using var reader = new StreamReader(response.Body, Encoding.UTF8, leaveOpen: true);
        responseBody = await reader.ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        _logger.LogInformation(
            "HTTP Response: {StatusCode} ({ElapsedMs}ms)\n" +
            "Headers: {Headers}\n" +
            "Body: {Body}",
            response.StatusCode,
            elapsedMilliseconds,
            FormatHeaders(response.Headers),
            responseBody);
    }

    private string FormatHeaders(IHeaderDictionary headers)
    {
        var formattedHeaders = new StringBuilder();
        foreach (var header in headers)
        {
            // 避免記錄敏感資訊
            if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
            {
                formattedHeaders.AppendLine($"  {header.Key}: [REDACTED]");
            }
            else
            {
                formattedHeaders.AppendLine($"  {header.Key}: {header.Value}");
            }
        }
        return formattedHeaders.ToString();
    }
}
