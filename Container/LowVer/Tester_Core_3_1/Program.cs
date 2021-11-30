using Benchmark_standard_2_0;
using BenchmarkDotNet.Running;

namespace Tester_Core_3_1
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(BenchmarkAssembly).Assembly).Run(args);
        }
    }
}
