using System;
using System.Diagnostics;

namespace Xero
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