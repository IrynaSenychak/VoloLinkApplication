namespace VoloLinkApp.Middleware
{
    public static class GeoBlockMiddlewareExtensions
    {
        public static IApplicationBuilder UseGeoBlock(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GeoBlockMiddleware>();
        }
    }
}