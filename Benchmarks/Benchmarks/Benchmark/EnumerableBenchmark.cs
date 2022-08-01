using System.Collections;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmark
{
    [MemoryDiagnoser]
    public class EnumerableFirstOrDefault
    {
        public class Container : IEnumerable<Input>
        {
            private readonly IEnumerable<Input> _list = new List<Input> {
                new Input("a"),
                new Input("a"),
                new Input("a"),
                new Input("a"),
                new Input("a"),
                new Input("a"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
            };

            public Input GetOrDefault(Predicate<Input> predicate)
            {
                foreach (var input in _list)
                {
                    if (predicate(input))
                    {
                        return input;
                    }
                }

                return null;
            }

            public IEnumerator<Input> GetEnumerator() => _list.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
        }

        private readonly Container container = new Container();

        [Benchmark]
        public void FirstOrDefault()
        {
            var _ = container.FirstOrDefault(x => x.Value == "a");
        }

        [Benchmark]
        public void PredicateForeach()
        {
            var _ = container.GetOrDefault(x => x.Value == "a");
        }
    }

    [MemoryDiagnoser]
    public class EnumerableRange
    {
        [Benchmark]
        public void Range()
        {
            foreach (var idx in Enumerable.Range(1, 12))
            {

            }
        }

        [Benchmark]
        public void For()
        {
            for (int idx = 1; idx <= 12; ++idx)
            {

            }
        }
    }
}
