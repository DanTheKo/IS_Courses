using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GatewayAPI.Services;

namespace GatewayAPI.PageFilters
{
    public class RedirectByAccessAttribute : Attribute, IAsyncPageFilter
    {
        public string ResourceIdRouteKey { get; } // Например, "id", "fileId", "postId"

        public string RedirectRoute { get; } // Например, "id", "fileId", "postId"

        public RedirectByAccessAttribute(string resourceIdRouteKey, string redirectRoute)
        {
            ResourceIdRouteKey = resourceIdRouteKey;
            RedirectRoute = redirectRoute;
        }
        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var identityId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(identityId))
            {
                context.Result = new RedirectToPageResult(RedirectRoute);
                return;
            }
            var resourceId = context.RouteData.Values[ResourceIdRouteKey]?.ToString();

            if (string.IsNullOrEmpty(resourceId))
            {
                context.Result = new RedirectToPageResult(RedirectRoute);
                return;
            }
            var accessService = context.HttpContext.RequestServices.GetRequiredService<AccessServiceClient>();

            try
            {
                var access = await accessService.GetAccessAsync(identityId, resourceId);
                if (!access.HasAccess)
                {
                    Console.WriteLine("Access denied. Redirecting to " + RedirectRoute);
                    context.Result = new RedirectToPageResult(RedirectRoute);
                    return;
                }
            }
            catch
            {
                context.Result = new RedirectToPageResult(RedirectRoute);
                return;
            }

            await next();
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }
    }
}
