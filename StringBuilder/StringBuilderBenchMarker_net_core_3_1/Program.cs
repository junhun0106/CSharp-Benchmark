using BenchmarkDotNet.Running;

namespace StringBuilderBenchMarker
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(StringBuilderBenchMark).Assembly).Run(args);
        }
    }
}
