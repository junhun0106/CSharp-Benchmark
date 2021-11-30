using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Benchmark_standard_2_0
{
    public static class BenchmarkAssembly { }

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

        readonly Dictionary<string, Data> _dictionary = new Dictionary<string, Data>();

        [GlobalSetup]
        public void GlobalSetUp()
        {
            for (int i = 0; i < 100; ++i)
            {
                var @string = i.ToString();
                _dictionary.Add(@string, new Data(@string));
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

    [MemoryDiagnoser]
    public class DictionaryValuesBoxingBenchmark
    {
        interface IData
        {

        }

        struct Data : IData
        {
            public readonly string id;

            public Data(string id)
            {
                this.id = id;
            }
        }

        readonly Dictionary<string, IData> _dictionary = new Dictionary<string, IData>();

        [GlobalSetup]
        public void GlobalSetUp()
        {
            for (int i = 0; i < 100; ++i)
            {
                var @string = i.ToString();
                _dictionary.Add(@string, new Data(@string));
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
