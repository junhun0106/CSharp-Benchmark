using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Tester
{
    [SimpleJob(RuntimeMoniker.Net50)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [MemoryDiagnoser]
    public class BanchmarkBase
    {
    }
}