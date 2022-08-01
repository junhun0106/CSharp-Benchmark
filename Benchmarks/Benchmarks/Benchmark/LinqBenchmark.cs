using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmark;

[MemoryDiagnoser]
public class SelectToListBenchmark
{
    private readonly Input[] _list = new Input[] {
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
    };

    [Benchmark]
    public void SelectToList()
    {
        var _ = _list.Select(x => new Output(x.Value)).ToList();
    }

    [Benchmark]
    public void ConvertAll()
    {
        var _ = _list.ConvertAll(x => new Output(x.Value));
    }
}

[MemoryDiagnoser]
public class WhereToListBenchmark
{
    private readonly Input[] _list = new Input[] {
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
    };

    [Benchmark]
    public void WhererToList()
    {
        var _ = _list.Where(x => x.Value == "a").ToList();
    }

    [Benchmark]
    public void ToListPredicate()
    {
        var _ = _list.ToList(x => x.Value == "a");
    }
}

[MemoryDiagnoser]
public class WhereSelectToListBenchmark
{
    private readonly Input[] _list = new Input[] {
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
        new Input("a"),
    };

    [Benchmark]
    public void WhererSelectToList()
    {
        var _ = _list.Where(x => x.Value == "a").Select(x => new Output(x.Value)).ToList();
    }

    [Benchmark]
    public void ToListPredicateConvertAll()
    {
        var _ = _list.ToList(x => x.Value == "a").ConvertAll(x => new Output(x.Value));
    }
}
