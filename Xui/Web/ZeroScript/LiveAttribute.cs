using System;
using System.Diagnostics;

namespace Xui.Web.ZeroScript
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class LiveAttribute : Attribute
    {
        public LiveAttribute()
        {
        }

        public string PropertyName { get; set; } = "";
    }
}