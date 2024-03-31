using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace B1WEB.ActionFilters
{
    public class SessionAuthorizationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if session exists
            if (context.HttpContext.Session.GetString("UserId") == null)
            {
                // Redirect to login page or perform any other action
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Implement if needed
        }
    }
}
