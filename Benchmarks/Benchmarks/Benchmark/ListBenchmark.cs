using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmark
{
    [Description("List<T>.First vs List<T>.Find")]
    public class ListFindBenchmark : ListBenchmarkBase
    {
        [Benchmark]
        public void FirstOrDefault()
        {
            var _ = _list.FirstOrDefault(x => x == "a");
        }

        [Benchmark]
        public void Find()
        {
            var _ = _list.Find(x => x == "a");
        }
    }

    public class ListAnyBenchmark : ListBenchmarkBase
    {
        [Benchmark]
        public void Any()
        {
            var _ = _list.Any();
        }

        [Benchmark]
        public void Count()
        {
            var _ = _list.Count > 0;
        }
    }

    public class ListFindNullVsAnyBenchmark : ListBenchmarkBase
    {
        [Benchmark]
        public void FindNull()
        {
            var _ = _list.Find(x => x == "a") != null;
        }

        [Benchmark]
        public void Any()
        {
            var _ = _list.Any(x => x == "a");
        }
    }

    public class List_Last : ListBenchmarkBase
    {
        [Benchmark]
        public void LastOrDefault()
        {
            var _ = _list.LastOrDefault();
        }

        [Benchmark]
        public void Index()
        {
            if (_list.Count > 0)
            {
                var _ = _list[_list.Count - 1];
            }
        }
    }

    public class List_First : ListBenchmarkBase
    {
        [Benchmark]
        public void FirstOrDefault()
        {
            var _ = _list.FirstOrDefault();
        }

        [Benchmark]
        public void Index0()
        {
            if (_list.Count > 0)
            {
                var _ = _list[0];
            }
        }
    }

    public class LinqContains : ListBenchmarkBase
    {
        [Benchmark]
        public void Contains_Linq()
        {
            if (_list.Contains("a"))
            {
            }
        }

        [Benchmark]
        public void Contains_LinqStringComparer()
        {
            if (_list.Contains("a", StringComparer.Ordinal))
            {
            }
        }

        [Benchmark]
        public void Contains_Foreach()
        {
            if (_list.CustomContains("a"))
            {
            }
        }
    }
}
