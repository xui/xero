namespace Xero;

using System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Field)]
public class LiveAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public class XeroPageAttribute : Attribute
{
    public XeroPageAttribute([StringSyntax("Route")] string route)
    {
    }
}