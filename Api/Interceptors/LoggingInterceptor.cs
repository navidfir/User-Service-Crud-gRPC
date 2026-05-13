using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Api.Interceptors;

public class LoggingInterceptor : Interceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(
        ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var method = context.Method;

        _logger.LogInformation(
            "gRPC Request {Method} {@Request}",
            method,
            request);

        var response = await continuation(request, context);

        _logger.LogInformation(
            "gRPC Response {Method}",
            method);

        return response;
    }
}