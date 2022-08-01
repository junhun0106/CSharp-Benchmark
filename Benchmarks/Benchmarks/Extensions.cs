namespace Benchmarks;

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

public static class DicionaryExtensions
{
    public static KeyValuePair<TKey, TValue> GetFirstOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source)
    {
        foreach (var kv in source)
        {
            return kv;
        }

        return default(KeyValuePair<TKey, TValue>);
    }
}

public static class EnumExtensions
    {
        public static List<TOutput> ConvertAll<TInput, TOutput>(this IReadOnlyList<TInput> source, Converter<TInput, TOutput> converter)
        {
            var list = new List<TOutput>(source.Count);
            foreach (var item in source)
            {
                list.Add(converter(item));
            }
            return list;
        }

        public static List<T> ToList<T>(this IReadOnlyList<T> source, Predicate<T> predicate)
        {
            if (source?.Count > 0)
            {
                var list = new List<T>(source.Count);
                foreach (var item in source)
                {
                    if (predicate(item))
                    {
                        list.Add(item);
                    }
                }
                return list;
            }

            return new List<T>();
        }

        public static bool CustomContains<T>(this IReadOnlyList<T> source, T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (source != null && source?.Count > 0)
            {
                foreach (var t in source)
                {
                    if (t.Equals(item))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
