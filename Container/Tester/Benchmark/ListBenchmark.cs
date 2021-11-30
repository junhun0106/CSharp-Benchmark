using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tester.Benchmark
{
    [MemoryDiagnoser]
    public class ListFindBenchmark
    {
        private readonly List<string> _list = new List<string> {
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

    [MemoryDiagnoser]
    public class ListAnyBenchmark
    {
        private readonly List<string> _list = new List<string> {
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

    [MemoryDiagnoser]
    public class ListFindNullVsAnyBenchmark
    {
        private readonly List<string> _list = new List<string> {
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

    [MemoryDiagnoser]
    public class ListLast
    {
        private readonly List<string> _list = new List<string> {
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

    [MemoryDiagnoser]
    public class ListIndex0
    {
        private readonly List<string> _list = new List<string> {
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

    [MemoryDiagnoser]
    public class LinqContains
    {
        private readonly string[] _list = new string[] {
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
