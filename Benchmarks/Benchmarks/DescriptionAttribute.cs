namespace Benchmarks
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DescriptionAttribute : Attribute
    {
        public readonly string Description;

        public DescriptionAttribute(string description) => Description = description;
    }
}
