using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using Presentation.Models;

namespace Presentation.Filters;

public class HeaderValidationFilter(IServiceProvider services) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ModelState.IsValid)
        {
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg is BaseWriteRequestDto writeDto)
                {
                    var validator = services.GetService<IValidator<BaseWriteRequestDto>>();
                    if (validator != null)
                    {
                       await validator.ValidateAndThrowAsync(writeDto, context.HttpContext.RequestAborted);
                    }
                }

                if (arg is BaseReadRequestDto readDto)
                {
                    var validator = services.GetService<IValidator<BaseReadRequestDto>>();
                    if (validator != null)
                    {
                        await validator.ValidateAndThrowAsync(readDto,  context.HttpContext.RequestAborted);
                    }
                }
            }
        }

        await next();
    }
}