using BenchmarkDotNet.Running;

namespace StringBuilderBenchMarker
{
    internal static class Program
    {
        private const string testString = "일이삼사오육칠팔구십일이삼사오육칠팔구십";

        private static void Main(string[] args)
        {
#if DEBUG
            var charArray = testString.ToCharArray();
            using var sb = new ValueStringBuilder(testString.Length * 2 * 4);
            sb.Append(testString);
            sb.Append(testString);
            sb.Append(testString);
            sb.Append(testString);
            sb.Append(testString);
#else
            BenchmarkSwitcher.FromAssembly(typeof(StringBuilderBenchMark).Assembly).Run(args);
#endif
            }
    }
}
