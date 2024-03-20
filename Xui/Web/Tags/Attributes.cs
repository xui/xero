using System.Diagnostics.CodeAnalysis;

namespace Xui.Web.HttpX;

[AttributeUsage(AttributeTargets.Method)]
public class HttpXPageAttribute : Attribute
{
    public HttpXPageAttribute([StringSyntax("Route")] string route)
    {
    }
}