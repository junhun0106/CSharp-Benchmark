using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmark
{
    public class ArrayFindBenchmark : ArrayBenchmarkBase
    {
        [Benchmark]
        public void ArrayFind()
        {
            var _ = _list.Find(x => x.Value == "a");
        }

        [Benchmark]
        public void FirstOrDefault()
        {
            var _ = _list.FirstOrDefault(x => x.Value == "a");
        }
    }

    [MemoryDiagnoser]
    public class ArrayFindAllBenchmark : ArrayBenchmarkBase
    {
        [Benchmark]
        public void ArrayFindAll()
        {
            var a = _list.FindAll(x => x.Value == "a");
        }

        [Benchmark]
        public void Where()
        {
            var a = _list.Where(x => x.Value == "a");
        }
    }

    [MemoryDiagnoser]
    public class XArrayContainsBenchmark : ArrayBenchmarkBase
    {
        private Input _input;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _input = new Input("c");
            _list = new Input[] {
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
                _input,
            };
        }

        [Benchmark]
        public void BinarySearch()
        {
            if (_list.Exists_3(_input))
            {
                //throw new Exception();
            }
            else
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void Foreach()
        {
            if (_list.Exists(_input))
            {
                //throw new Exception();
            }
            else
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void For()
        {
            if (_list.Exists_2(_input))
            {
                //throw new Exception();
            }
            else
            {
                throw new Exception();
            }
        }

        //[Benchmark]
        //public void ForStringOrdinalComparer()
        //{
        //    if (_list.Exists_2(_input)) {
        //        throw new Exception();
        //    }
        //}

        [Benchmark]
        public void ContainsComparerNull()
        {
            if (_list.Contains(_input, comparer: null))
            {
                //throw new Exception();
            }
            else
            {
                throw new Exception();
            }
        }

        //[Benchmark]
        //public void ContainsComparerStringOrdinal()
        //{
        //    if (_list.Contains(_input, StringComparer.Ordinal)) {
        //        throw new Exception();
        //    }
        //}
    }
}
