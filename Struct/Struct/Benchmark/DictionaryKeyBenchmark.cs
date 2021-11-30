using System.Linq;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using System;

namespace Struct.Benchmark
{
    [MemoryDiagnoser]
    [BenchmarkDescription("Dictionary의 key로 struct가 들어갔을 경우 처리에 따른 벤치마크")]
    public class DictionaryKeyBenchmark : IValidate
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
                throw new Exception("DictionaryKeyBenchmark.Invalidate");
            }

            if (!_dic_comparer.ContainsKey(_comparer))
            {
                throw new Exception("DictionaryKeyBenchmark.Invalidate");
            }
        }
    }
}
