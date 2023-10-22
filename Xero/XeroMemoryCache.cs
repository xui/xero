using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Xero;

public abstract partial class UI<T>
{
    internal class XeroMemoryCache
    {
        private const string SESSION_KEY = "xero_session";
        private static MemoryCache cache = new(new MemoryCacheOptions());
        private static MemoryCacheEntryOptions entryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(1));

        public static Context Get(HttpContext httpContext, UI<T> ui)
        {
            var sessionId = GetSessionId(httpContext);
            var xeroContext = cache.Get(sessionId) as Context;
            if (xeroContext == null)
            {
                xeroContext = new Context(ui);
                Set(sessionId, xeroContext);
            }
            return xeroContext;
        }

        private static string GetSessionId(HttpContext httpContext)
        {
            var sessionId = httpContext.Request.Cookies[SESSION_KEY];
            if (sessionId == null)
            {
                sessionId = Guid.NewGuid().ToString();
                httpContext.Response.Cookies.Append(SESSION_KEY, sessionId);
                // TODO: Expire cookie?
            }
            return sessionId;
        }

        private static void Set(string id, Context context)
        {
            cache.Set(id, context, entryOptions);
        }
    }
}