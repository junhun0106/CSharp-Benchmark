using System;

namespace Struct
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BenchmarkDescriptionAttribute : Attribute
    {
        public readonly string Description;

        public BenchmarkDescriptionAttribute(string description) => Description = description;
    }
}
