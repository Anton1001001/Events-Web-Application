using EventsWebApplication.Application.Helpers;
using Microsoft.AspNetCore.Http;

namespace EventsWebApplication.Infrastructure.Authorization;

public class CookieProvider : ICookieProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetCookie(string key, string value, bool isHttpOnly = true, bool isSecure = true)
    {
        var options = new CookieOptions
        {
            HttpOnly = isHttpOnly,
            Secure = isSecure,
            IsEssential = true,
        };

        _httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, options);
    }

    public string GetCookie(string key)
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies[key];
    }

    public void RemoveCookie(string key)
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
    }
}
