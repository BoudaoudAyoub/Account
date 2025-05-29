using Account.Domain.Exceptions;
using Account.Domain.Exceptions.Models;
using Account.SharedKernel.HttpReponses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Account.API.Infrastructure.Filters;

public class CustomExceptionFilter(ILogger<CustomExceptionFilter> exceptionFilterLogger) : IExceptionFilter
{
    private readonly ILogger<CustomExceptionFilter> _exceptionFilterLogger = exceptionFilterLogger;

    public void OnException(ExceptionContext context)
    {
        // Check whether the exception is handled by any of the filters in the application
        if (context is null || context.ExceptionHandled || context.Exception is NullReferenceException) return;

        ExceptionRoot exceptionRoot = new()
        {
            ExceptionLogTime = DateTime.UtcNow,
            Instance = context.HttpContext.Request.Path,
            ActionName = context.RouteData.Values["action"]?.ToString() ?? string.Empty,
            ControllerName = context.RouteData.Values["controller"]?.ToString() ?? string.Empty
        };

        if (context.Exception is DomainException exception)
        {
            exceptionRoot.ErrorResponse = (exception.Errors == default! || exception.Errors.Count == 0) ? 
            new()
            {
                StatusCode = exception.StatusCode,
                Error = exception.Message
            }
            :
            new()
            {
                Errors = exception.Errors?.Select(exception => new Errors()
                {
                    StatusCode = exception.Key,
                    _Errors = exception.Value.ToArray()

                }).ToList()
            };
        }
        else
        {
            exceptionRoot.ErrorResponse = new()
            {
                StatusCode = HttpResponseType.InternalServerError,
                Error = string.Format($"{HttpResponseMessageType.InternalServerError} : {context.Exception.Message}")
            };
        }

        context.ExceptionHandled = true;
        context.Result = new ObjectResult(new
        {
            Status = HttpResponseJsonType.Failure,
            Failures = exceptionRoot 
        });
        _exceptionFilterLogger.LogError(new EventId(context.Exception.HResult), string.Format("@exceptionRoot", exceptionRoot));
    }
}