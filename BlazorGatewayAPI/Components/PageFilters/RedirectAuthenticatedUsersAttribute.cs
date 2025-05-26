using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGatewayAPI.Components.PageFilters
{
    public class RedirectAuthenticatedUsersAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.HttpContext.User.Identity != null && context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToPageResult("/index");
            }

            base.OnResultExecuting(context);
        }
    }
}
