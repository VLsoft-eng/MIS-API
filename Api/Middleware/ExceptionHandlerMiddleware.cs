using System.ComponentModel.DataAnnotations;
using Application.Dto;
using Application.Exceptions;

namespace Api.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next.Invoke(httpContext);
        }
        catch (ValidationException e)
        {
            var error = new ErrorResponse(StatusCodes.Status400BadRequest, e.Message);
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (AlreadyLogoutException e)
        {
            var error = new ErrorResponse(StatusCodes.Status400BadRequest, e.Message);
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (DoctorNotFoundException e)
        {
            var error = new ErrorResponse(StatusCodes.Status404NotFound, e.Message);
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (EmailAlreadyUsedException e)
        {
            var error = new ErrorResponse(StatusCodes.Status400BadRequest, e.Message);
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (InvalidAuthCredentialsException e)
        {
            var error = new ErrorResponse(StatusCodes.Status400BadRequest, e.Message);
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (SpecialityNotFoundException e)
        {
            var error = new ErrorResponse(StatusCodes.Status404NotFound, e.Message);
            await httpContext.Response.WriteAsJsonAsync(error);
        }
    }
}