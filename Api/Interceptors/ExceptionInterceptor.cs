using Application.Common.Exceptions;
using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Api.Interceptors;

public class ExceptionInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (ValidationException ex)
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    ex.Message));
        }
        catch (NotFoundException ex)
        {
            throw new RpcException(
                new Status(
                    StatusCode.NotFound,
                    ex.Message));
        }
        catch (AlreadyExistsException ex)
        {
            throw new RpcException(
                new Status(
                    StatusCode.AlreadyExists,
                    ex.Message));
        }
        catch (ConcurrencyException ex)
        {
            throw new RpcException(
                new Status(
                    StatusCode.Aborted,
                    ex.Message));
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new RpcException(
                new Status(
                    StatusCode.Aborted,
                    "Database concurrency conflict"));
        }
        catch (DbUpdateException ex)
        {
            throw new RpcException(
                new Status(
                    StatusCode.Internal,
                    $"Database error: {ex.InnerException?.Message ?? ex.Message}"));
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new RpcException(
                new Status(
                    StatusCode.Internal,
                    "Internal server error"));
        }
    }
}