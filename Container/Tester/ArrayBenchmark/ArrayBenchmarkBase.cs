using System;
using System.Collections.Generic;

namespace Tester.ArrayBenchmark
{
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

    public class ArrayBenchmarkBase : BanchmarkBase
    {
        protected Input[] _list = new Input[] {
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
    }
}