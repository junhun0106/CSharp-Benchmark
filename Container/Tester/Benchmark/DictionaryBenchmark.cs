using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tester.Benchmark
{
    [MemoryDiagnoser]
    public class DictionryLookUp
    {
        private readonly Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["a"] = "a",
            ["b"] = "b",
            ["c"] = "c",
            ["d"] = "d",
            ["e"] = "e",
            ["f"] = "f",
        };

        [Benchmark]
        public void ContainsKeyIndex()
        {
            if (_dic.ContainsKey("a"))
            {
                var _ = _dic["a"];
            }
        }

        [Benchmark]
        public void TryGetValue()
        {
            if (_dic.TryGetValue("a", out var _))
            {

            }
        }

    }

    [MemoryDiagnoser]
    public class DictionaryWhereBenchmark
    {
        private readonly Dictionary<string, Input> _list = new Dictionary<string, Input>(StringComparer.Ordinal)
        {
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
        };

        [Benchmark]
        public void Foreach()
        {
            foreach (var kv in _list)
            {
                if (kv.Value.Value == "a")
                {
                }
            }
        }

        [Benchmark]
        public void Where()
        {
            var wheres = _list.Where(kv => kv.Value.Value == "a");
            foreach (var item in wheres)
            {

            }
        }
    }

    [MemoryDiagnoser]
    public class DictionaryElementAt
    {
        private readonly Dictionary<string, Input> _list = new Dictionary<string, Input>(StringComparer.Ordinal)
        {
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
        };

        [Benchmark]
        public void ElementAt()
        {
            var _ = _list.ElementAt(0);
        }

        [Benchmark]
        public void ElementAtOrDefault()
        {
            var _ = _list.ElementAtOrDefault(0);
        }

        [Benchmark]
        public void First()
        {
            var _ = _list.First();
        }

        [Benchmark]
        public void FirstOrDefault()
        {
            var _ = _list.FirstOrDefault();
        }

        [Benchmark]
        public void Foreach()
        {
            var _ = _list.GetFirstOrDefault();
        }
    }

    public static class DicionaryExtensions
    {
        public static KeyValuePair<TKey, TValue> GetFirstOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source)
        {
            foreach (var kv in source)
            {
                return kv;
            }

            return default(KeyValuePair<TKey, TValue>);
        }
    }

    [MemoryDiagnoser]
    public class DictionaryValuesBenchmark
    {
        class Data
        {
            public readonly string id;

            public Data(string id)
            {
                this.id = id;
            }
        }

        readonly Dictionary<string, Data> _dictionary = new();

        private Dictionary<string, Data>.ValueCollection _values => _dictionary.Values;

        private IEnumerable<Data> _values2 => _dictionary.Values;

        [GlobalSetup]
        public void GlobalSetUp()
        {
            for (int i = 0; i < 1000; ++i)
            {
                var @string = (i % 10).ToString();
                _dictionary.Add(i.ToString(), new Data(@string));
            }
        }

        [Benchmark]
        public void Values()
        {
            foreach (var value in _values)
            {
            }
        }

        [Benchmark]
        public void Values2()
        {
            foreach (var value in _values2)
            {
            }
        }

        [Benchmark]
        public void Foreach()
        {
            foreach (var kv in _dictionary)
            {
            }
        }

        [Benchmark]
        public void ValueWhere()
        {
            var c = _values.Where(x => x.id == "0");
            foreach (var _ in c)
            {
            }
        }

        [Benchmark]
        public void Value2Where()
        {
            var c = _values2.Where(x => x.id == "0");
            foreach (var _ in c)
            {
            }
        }

        [Benchmark]
        public void ForeachWhere()
        {
            var c = _dictionary.Where(x => x.Value.id == "0");
            foreach (var _ in c)
            {
            }
        }
    }

    [MemoryDiagnoser]
    public class DictionaryStringComparerBenchmark
    {
        private readonly Dictionary<string, string> _dictionary1 = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _dictionary2 = new Dictionary<string, string>(StringComparer.Ordinal);

        [GlobalSetup]
        public void GlobalSetUp()
        {
            for (int i = 0; i < 100; ++i)
            {
                _dictionary1.Add(i.ToString(), i.ToString());
                _dictionary2.Add(i.ToString(), i.ToString());

            }
        }

        [Benchmark]
        public void DefaultComparer()
        {
            for (int i = 0; i < 100; ++i)
                _dictionary1.TryGetValue("100", out var _);
        }

        [Benchmark]
        public void StringOrdinalComparer()
        {
            for (int i = 0; i < 100; ++i)
                _dictionary2.TryGetValue("100", out var _);
        }
    }
}
