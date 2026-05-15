using LayerZero.Tools.Web.Bundles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace LayerZero.Tools.Web.Middleware
{
    public class BundleServingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly BundleStore _store;

        public BundleServingMiddleware(RequestDelegate next, BundleStore store)
        {
            _next = next;
            _store = store;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            if (!path.StartsWith("/bundles", StringComparison.OrdinalIgnoreCase)
                || !_store.IsRegistered(path))
            {
                await _next(context);
                return;
            }

            if (!_store.TryGetOrBuild(path, out var bundle))
            {
                await _next(context);
                return;
            }

            context.Response.ContentType = bundle.ContentType;
            context.Response.Headers.ETag = bundle.ETag;
            context.Response.Headers.CacheControl = "public,max-age=31536000";
            await context.Response.WriteAsync(bundle.Content);
        }
    }

    public static class BundleServingMiddlewareExtensions
    {
        public static IApplicationBuilder UseBundleServing(this IApplicationBuilder app)
            => app.UseMiddleware<BundleServingMiddleware>();
    }
}
