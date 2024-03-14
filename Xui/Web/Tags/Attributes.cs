using System.Diagnostics.CodeAnalysis;

namespace Xui.Web.Tags;

[AttributeUsage(AttributeTargets.Method)]
public class XeroPageAttribute : Attribute
{
    public XeroPageAttribute([StringSyntax("Route")] string route)
    {
    }
}