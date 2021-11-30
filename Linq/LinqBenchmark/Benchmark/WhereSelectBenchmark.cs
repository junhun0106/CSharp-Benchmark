using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace LinqBenchmark.Benchmark
{
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
        public void WhereSelectLinq()
        {
            _ = _list.Where(x => x.@string == "0").Select(x => x.@string);
        }

        [Benchmark]
        public void WhereSelectCustom()
        {
            static bool where(A x) => x.@string == "0";
            static string select(A x) => x.@string;
            _ = _list.WhereSelect(x => where(x), x => select(x));
        }
    }
}
