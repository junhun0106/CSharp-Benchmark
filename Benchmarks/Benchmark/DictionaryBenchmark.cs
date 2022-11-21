using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;

// Wiki - https://github.com/junhun0106/CSharp/wiki/%5BBenchmark%5D-Dictionary
namespace Benchmarks.Benchmark
{
    // https://learn.microsoft.com/ko-kr/dotnet/fundamentals/code-analysis/quality-rules/ca1836
    public class DictionryLookUp : BenchmarkBase
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

    [Description("Dictionary<TKey> : TKey where struct")]
    public class DictionaryStructKeyTryGetValue : BenchmarkBase, IValidate
    {
        private readonly TestStruct _comparer = new(1, EType.B, new DateTime(2021, 11, 30), "1");
        private readonly Dictionary<TestStruct, string> _dic = new();
        private readonly Dictionary<TestStruct, string> _dic_comparer = new(TestStructEqulityComparer.Default);

        [GlobalSetup]
        public void GlobalSetUp()
        {
            foreach (var i in Enumerable.Range(1, 100))
            {
                _dic.Add(new TestStruct(i, (EType)(i % 4), new DateTime(2021, 11, 30), i.ToString()), i.ToString());
                _dic_comparer.Add(new TestStruct(i, (EType)(i % 4), new DateTime(2021, 11, 30), i.ToString()), i.ToString());
            }
        }

        [Benchmark]
        public void TryGetValue()
        {
            foreach (var i in Enumerable.Range(1, 100))
                _dic.TryGetValue(_comparer, out var _);
        }

        [Benchmark]
        public void TryGetValueComparer()
        {
            foreach (var i in Enumerable.Range(1, 100))
                _dic_comparer.TryGetValue(_comparer, out var _);
        }

        public void Validate()
        {
            foreach (var i in Enumerable.Range(1, 100))
            {
                _dic.Add(new TestStruct(i, (EType)(i % 4), new DateTime(2021, 11, 30), i.ToString()), i.ToString());
                _dic_comparer.Add(new TestStruct(i, (EType)(i % 4), new DateTime(2021, 11, 30), i.ToString()), i.ToString());
            }

            if (!_dic.ContainsKey(_comparer))
            {
                throw new Exception($"{nameof(DictionaryStructKeyTryGetValue)}.Invalidate");
            }

            if (!_dic_comparer.ContainsKey(_comparer))
            {
                throw new Exception($"{nameof(DictionaryStructKeyTryGetValue)}.Invalidate");
            }
        }
    }

    public class DictionaryStringKeyTryGetValue : BenchmarkBase
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

    public class DictionaryWhereBenchmark : BenchmarkBase
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

    public class DictionaryElementAt : BenchmarkBase
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

    public class DictionaryValuesBenchmark : BenchmarkBase
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
            foreach (var value in _dictionary.Values)
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
    }

    // https://github.com/junhun0106/CSharp/wiki/%5BBenchmark%5D-ConcurrentDictionary
    public class ConcurrentDictionaryEmptyBenchmark : BenchmarkBase
    {
        private static ConcurrentDictionary<string, string> dic = new();

        [Benchmark]
        public void IsEmpty()
        {
            if (dic.IsEmpty)
            {

            }
        }

        [Benchmark]
        public void Count()
        {
            if (dic.Count <= 0)
            {

            }
        }
    }

    public class ConcurrentDictionaryNotEmptyBenchmark : BenchmarkBase
    {
        private static ConcurrentDictionary<string, string> dic = new()
        {
            ["a"] = "a",
            ["b"] = "b",
            ["c"] = "b",
            ["d"] = "b",
            ["e"] = "b",
            ["f"] = "b",
            ["g"] = "b",
            ["1"] = "b",
            ["2"] = "b",
            ["3"] = "b",
            ["4"] = "b",
            ["5"] = "b",
            ["6"] = "b",
            ["7"] = "b",
            ["8"] = "b",
            ["9"] = "b",
        };

        [Benchmark]
        public void IsEmpty()
        {
            if (dic.IsEmpty)
            {

            }
        }

        [Benchmark]
        public void Count()
        {
            if (dic.Count <= 0)
            {

            }
        }
    }
}
