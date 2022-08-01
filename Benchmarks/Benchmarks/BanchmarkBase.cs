using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks
{
    [SimpleJob(RuntimeMoniker.Net50)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [MemoryDiagnoser]
    public class BanchmarkBase
    {
    }

    public class ArrayBenchmarkBase : BanchmarkBase
    {
        protected Input[] _list = new Input[] {
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
        };
    }
}