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
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (AlreadyLogoutException e)
        {
            var error = new ErrorResponse(StatusCodes.Status400BadRequest, e.Message);
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (DoctorNotFoundException e)
        {
            var error = new ErrorResponse(StatusCodes.Status404NotFound, e.Message);
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (EmailAlreadyUsedException e)
        {
            var error = new ErrorResponse(StatusCodes.Status400BadRequest, e.Message);
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (InvalidAuthCredentialsException e)
        {
            var error = new ErrorResponse(StatusCodes.Status400BadRequest, e.Message);
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (SpecialityNotFoundException e)
        {
            var error = new ErrorResponse(StatusCodes.Status404NotFound, e.Message);
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (InvalidPaginationParamsException e)
        {
            var error = new ErrorResponse(StatusCodes.Status400BadRequest, e.Message);
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (InspectionNotFoundException e)
        {
            var error = new ErrorResponse(StatusCodes.Status404NotFound, e.Message);
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (CommentNotFoundException e)
        {
            var error = new ErrorResponse(StatusCodes.Status404NotFound, e.Message);
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (ConsultationNotFoundException e)
        {
            var error = new ErrorResponse(StatusCodes.Status404NotFound, e.Message);
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (IcdNotFoundException e)
        {
            var error = new ErrorResponse(StatusCodes.Status404NotFound, e.Message);
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (PatientNotFoundException e)
        {
            var error = new ErrorResponse(StatusCodes.Status404NotFound, e.Message);
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(error);
        }
    }
}