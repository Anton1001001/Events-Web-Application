namespace EventsWebApplication.Application.Helpers;

public interface ICookieProvider
{
    void SetCookie(string key, string value, bool isHttpOnly = true, bool isSecure = true);
    string GetCookie(string key);
    void RemoveCookie(string key);
}