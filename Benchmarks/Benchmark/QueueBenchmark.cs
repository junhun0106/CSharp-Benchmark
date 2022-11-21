using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmark
{
    public class QueueEmptyBenchmark : BenchmarkBase
    {
        static ConcurrentQueue<string> container = new();

        [Benchmark]
        public void IsEmpty()
        {
            if (container.IsEmpty)
            {

            }
        }

        [Benchmark]
        public void Count()
        {
            if (container.Count <= 0)
            {

            }
        }
    }

    public class QueueNotEmptyBenchmark : BenchmarkBase
    {
        static ConcurrentQueue<string> container = new();

        [GlobalSetup]
        public void Setup()
        {
            container.Enqueue("a");
            container.Enqueue("b");
            container.Enqueue("c");
            container.Enqueue("d");
        }

        [Benchmark]
        public void IsEmpty()
        {
            if (container.IsEmpty)
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void Count()
        {
            if (container.Count <= 0)
            {
                throw new Exception();
            }
        }
    }
}
