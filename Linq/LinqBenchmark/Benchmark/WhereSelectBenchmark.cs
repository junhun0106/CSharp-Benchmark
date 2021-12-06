using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace LinqBenchmark.Benchmark
{
    [MemoryDiagnoser]
    public class WhereSelectBenchmark
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
        private readonly List<A> _list = new (_capacity);

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
}
