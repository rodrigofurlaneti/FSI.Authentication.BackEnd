using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FSI.Authentication.Presentation.Filters
{
    public sealed class ModelStateValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var problem = new ValidationProblemDetails(context.ModelState)
                {
                    Title = "Erro de validação",
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://httpstatuses.com/400",
                    Instance = context.HttpContext.Request.Path
                };
                context.Result = new ObjectResult(problem)
                {
                    StatusCode = problem.Status,
                    ContentTypes = { "application/problem+json" }
                };
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
