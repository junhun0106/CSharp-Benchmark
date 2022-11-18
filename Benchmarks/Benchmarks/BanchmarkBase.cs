using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks
{
    [SimpleJob(RuntimeMoniker.Net70)]
    [MemoryDiagnoser]
    public class BenchmarkBase
    {
    }

    public class ArrayBenchmarkBase : BenchmarkBase
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

    public class ListBenchmarkBase : BenchmarkBase
    {
        protected readonly List<string> _list = new List<string> {
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
        };
    }
}