using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace LinqBenchmark.Benchmark
{
    /// <summary>
    /// IEnumable로 가져와서 list.Add vs list.Capacity = IEnumable.Count() + list.Add 비교
    /// </summary>
    [MemoryDiagnoser]
    public class CountBenchmark
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
            foreach(var value in collection)
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
}
