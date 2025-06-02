using MaxMind.GeoIP2;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace VoloLinkApp.Middleware
{
    public class GeoBlockMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DatabaseReader _reader;

        public GeoBlockMiddleware(RequestDelegate next)
        {
            _next = next;
            
            _reader = new DatabaseReader("GeoLite2-Country.mmdb");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress;
            
            if (ip != null && !ip.IsPrivateAddress())
            {
                try
                {
                    var response = _reader.Country(ip);
                    if (response.Country.IsoCode == "RU")
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("Access denied based on your location.");
                        return;
                    }
                }
                catch (Exception)
                {
                   
                }
            }

            await _next(context);
        }
    }

    public static class IpAddressExtensions
    {
        public static bool IsPrivateAddress(this IPAddress address)
        {
            if (address.IsIPv4MappedToIPv6)
            {
                address = address.MapToIPv4();
            }
            byte[] bytes = address.GetAddressBytes();
            return bytes[0] switch
            {
                10 => true,
                172 => bytes[1] >= 16 && bytes[1] < 32,
                192 => bytes[1] == 168,
                _ => false
            };
        }
    }
}