using FluentValidation;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace Roomy.Search.Filters;

/// <summary>
/// Action filter that automatically validates request DTOs using FluentValidation.
/// Discovers the appropriate validator from the DI container and validates the first
/// non-CancellationToken parameter. Throws ValidationException on failure.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ValidateRequestAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Get the action descriptor
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        if (actionDescriptor == null)
        {
            await next();
            return;
        }

        // Find the first parameter that is not a CancellationToken (this is the DTO)
        var parameters = actionDescriptor.MethodInfo.GetParameters();
        var dtoParameter = parameters.FirstOrDefault(p => p.ParameterType != typeof(CancellationToken));

        if (dtoParameter == null)
        {
            await next();
            return;
        }

        // Get the DTO value from action arguments
        if (!context.ActionArguments.TryGetValue(dtoParameter.Name!, out var dtoValue) || dtoValue == null)
        {
            await next();
            return;
        }

        // Get the validator type for this DTO: IValidator<TDto>
        var dtoType = dtoParameter.ParameterType;
        var validatorType = typeof(IValidator<>).MakeGenericType(dtoType);

        // Resolve the validator from DI container
        var validator = context.HttpContext.RequestServices.GetService(validatorType);
        if (validator == null)
        {
            // No validator registered, continue without validation
            await next();
            return;
        }

        // Get CancellationToken from action arguments if available
        var cancellationToken = CancellationToken.None;
        if (context.ActionArguments.TryGetValue("cancellationToken", out var ctValue) && ctValue is CancellationToken ct)
        {
            cancellationToken = ct;
        }

        // Invoke ValidateAsync using reflection
        var validateAsyncMethod = validatorType.GetMethod(
            "ValidateAsync",
            BindingFlags.Public | BindingFlags.Instance,
            null,
            new[] { dtoType, typeof(CancellationToken) },
            null
        );

        if (validateAsyncMethod != null)
        {
            var resultTask = validateAsyncMethod.Invoke(validator, new[] { dtoValue, cancellationToken }) as Task;
            
            if (resultTask != null)
            {
                await resultTask;

                // Get the Result property from the Task<ValidationResult>
                var resultProperty = resultTask.GetType().GetProperty(
                    "Result",
                    BindingFlags.Public | BindingFlags.Instance
                );

                if (resultProperty != null)
                {
                    var validationResultObj = resultProperty.GetValue(resultTask);
                    if (validationResultObj != null)
                    {
                        dynamic validationResult = validationResultObj;
                        if (!validationResult.IsValid)
                        {
                            // Throw ValidationException if validation failed
                            throw new ValidationException(validationResult.Errors);
                        }
                    }
                }
            }
        }

        // Validation passed or no validator found, continue to the action
        await next();
    }
}
