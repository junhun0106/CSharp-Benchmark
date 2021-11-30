using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Tester.Benchmark
{
    [MemoryDiagnoser]
    public class ArrayFindBenchmark
    {
        private readonly Input[] _list = new Input[] {
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
    public class ArrayFindAllBenchmark
    {
        private readonly Input[] _list = new Input[] {
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
    public class XArrayContainsBenchmark
    {
        private Input[] _list;

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

    public static class ArrayExntensions
    {
        public static T Find<T>(this T[] array, Predicate<T> pred)
        {
            return Array.Find(array, pred);
        }

        public static T[] FindAll<T>(this T[] array, Predicate<T> match)
        {
            return Array.FindAll(array, match);
        }

        public static int BinarySearch<T>(this T[] array, T value)
        {
            return Array.BinarySearch(array, value);
        }

        public static bool Exists<T>(this T[] array, T value)
        {
            foreach (var item in array)
            {
                if (item.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Exists_2<T>(this T[] array, T value, IEqualityComparer<T> comparer = null)
        {
            if (comparer == null)
            {
                for (int i = 0; i < array.Length; ++i)
                {
                    var item = array[i];
                    if (item.Equals(value))
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < array.Length; ++i)
                {
                    var item = array[i];
                    if (comparer.Equals(item, value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool Exists_3<T>(this T[] array, T value)
        {
            return Array.BinarySearch(array, value) > 0;
        }
    }
}
