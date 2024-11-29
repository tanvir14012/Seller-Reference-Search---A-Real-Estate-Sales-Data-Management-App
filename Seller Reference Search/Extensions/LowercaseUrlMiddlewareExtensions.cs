using Seller_Reference_Search.Models.Utility;

namespace Seller_Reference_Search.Extensions
{
    public static class LowercaseUrlMiddlewareExtensions
    {
        public static IApplicationBuilder UseLowercaseUrls(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LowercaseUrlMiddleware>();
        }

    }
}
