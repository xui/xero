using System;
using System.Diagnostics;

namespace Xero
{
    namespace SourceGenerators
    {
        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
        [Conditional("LiveGenerator_DEBUG")]
        public sealed class LiveAttribute : Attribute
        {
            public LiveAttribute()
            {
            }

            public string PropertyName { get; set; }
        }
    }
}