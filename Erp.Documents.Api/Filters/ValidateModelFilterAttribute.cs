using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Erp.Documents.Api.Filters
{
    /// <summary>
    /// Filtro de acción para validar automáticamente DTOs usando FluentValidation.
    /// Se ejecuta antes de que la acción del controller se ejecute.
    /// </summary>
    public class ValidateModelFilterAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

                context.Result = new BadRequestObjectResult(new
                {
                    message = "Errores de validación",
                    errors = errors,
                    timestamp = DateTime.UtcNow,
                    traceId = context.HttpContext.TraceIdentifier
                });

                return;
            }

            await next();
        }
    }
}
