using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SurveyApp.Core.Exceptions;

namespace SurveyApp.Api.Filters;

public sealed class DomainValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not DomainValidationException ex)
            return;

        var problem = new ValidationProblemDetails(ex.Errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation error",
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
        };

        context.Result = new BadRequestObjectResult(problem)
        {
            ContentTypes = { "application/problem+json" }
        };

        context.ExceptionHandled = true;
    }
}
