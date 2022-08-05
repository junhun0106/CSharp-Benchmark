using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmark;

[Description("List<T>.Add vs List<T>(IEnumerable.Count()).Add()")]
public class LinqCountBenchmark : BenchmarkBase
{
    private const int _testCount = 1000;
    private readonly List<string> _list = new List<string>(_testCount);

    [GlobalSetup]
    public void GlobalSetUp()
    {
        for (int i = 0; i < 1000; ++i)
        {
            _list.Add((i % 10).ToString());
        }
    }

    [Benchmark]
    public void Add()
    {
        var collection = _list.Where(x => x == "0");
        var list = new List<string>();
        foreach (var value in collection)
        {
            list.Add(value);
        }
    }

    [Benchmark]
    public void CountAdd()
    {
        var collection = _list.Where(x => x == "0");
        var list = new List<string>(collection.Count());
        foreach (var value in collection)
        {
            list.Add(value);
        }
    }
}

public class LinqSelectToListBenchmark : BenchmarkBase
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

public class LinqWhereSelectBenchmark : BenchmarkBase
{
    private class A
    {
        public readonly string @string;

        public A(string @string)
        {
            this.@string = @string;
        }
    }

    private const int _capacity = 1000;
    private readonly List<A> _list = new(_capacity);

    [GlobalSetup]
    public void GlobalSetUp()
    {
        for (int i = 0; i < _capacity; ++i)
        {
            _list.Add(new A((i % 10).ToString()));
        }
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void WhereSelectLinq()
    {
        var values = _list.Where(x => x.@string == "0").Select(x => x.@string);
        foreach (var value in values)
        {
        }
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void WhereSelectYield()
    {
        var values = _list.WhereSelect(x => x.@string == "0", x => x.@string);
        foreach (var value in values)
        {
        }
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void WhereSelectEnumerableInterator()
    {
        var values = _list.WhereSelect_2(x => x.@string == "0", x => x.@string);
        foreach (var value in values)
        {
        }
    }

    public void Validate()
    {
        var list = new List<A>(_capacity);
        for (int i = 0; i < _capacity; ++i)
        {
            _list.Add(new A((i % 10).ToString()));
        }

        var newList = new List<string>(list.Count);

        var values = _list.WhereSelect_2(x => x.@string == "0", x => x.@string);
        foreach (var value in values)
        {
            newList.Add(value);
        }
    }
}

public class LinqWhereToListBenchmark : BenchmarkBase
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

public class LinqWhereSelectToListBenchmark : BenchmarkBase
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

