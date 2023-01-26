using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

    public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> createFactory)
    where TKey : notnull
    {
        ref TValue val = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
        if (!exists)
        {
            val = createFactory();
        }

        return val;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetValueRef<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, out TValue value)
    {
        ref TValue val = ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);
        value = val;
        return !Unsafe.IsNullRef(ref val);
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

public static class StructExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasValue(this TestStruct self)
    {
        return !self.IsNull();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNull(this TestStruct self)
    {
        return self.Equals(default(TestStruct));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasValue(this TestStruct_NotImpl self)
    {
        return !self.IsNull_2();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNull_2(this TestStruct_NotImpl self)
    {
        return self.Equals(default(TestStruct_NotImpl));
    }
}

public static partial class EnumerableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TResult> WhereSelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        foreach (var item in source)
        {
            if (predicate(item))
            {
                yield return selector(item);
            }
        }
    }

    /// <summary>
    /// An iterator that filters, then maps, each item of an <see cref="IEnumerable{TSource}"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
    /// <typeparam name="TResult">The type of the mapped items.</typeparam>
    private class WhereSelectEnumerableIterator<TSource, TResult> : IEnumerable<TResult>, IEnumerator<TResult>
    {
        #region class Iterator
        // https://source.dot.net/#System.Linq/System/Linq/Iterator.cs,00f3550fbfefd123
        private readonly int _threadId;
        private int _state;
        private TResult _current;
        /// <summary>
        /// The item currently yielded by this iterator.
        /// </summary>
        public TResult Current => _current;

        public IEnumerator<TResult> Clone() => new WhereSelectEnumerableIterator<TSource, TResult>(_source, _predicate, _selector);

        public void Dispose()
        {
            if (_enumerator != null)
            {
                _enumerator.Dispose();
                _enumerator = null;
            }

            _current = default!;
            _state = -1;
        }

        /// <summary>
        /// Gets the enumerator used to yield values from this iterator.
        /// </summary>
        /// <remarks>
        /// If <see cref="GetEnumerator"/> is called for the first time on the same thread
        /// that created this iterator, the result will be this iterator. Otherwise, the result
        /// will be a shallow copy of this iterator.
        /// </remarks>
        public IEnumerator<TResult> GetEnumerator()
        {
            IEnumerator<TResult> enumerator = _state == 0 && _threadId == Environment.CurrentManagedThreadId ? this : Clone();
            _state = 1;
            return enumerator;
        }

        object IEnumerator.Current => Current;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void IEnumerator.Reset() => throw new NotImplementedException();
        #endregion

        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, bool> _predicate;
        private readonly Func<TSource, TResult> _selector;
        private IEnumerator<TSource> _enumerator;

        public WhereSelectEnumerableIterator(IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
        {
            System.Diagnostics.Debug.Assert(source != null);
            System.Diagnostics.Debug.Assert(predicate != null);
            System.Diagnostics.Debug.Assert(selector != null);
            _source = source;
            _predicate = predicate;
            _selector = selector;
            _enumerator = null;
            _state = 0;
            _current = default;
            _threadId = Environment.CurrentManagedThreadId;
        }

        public bool MoveNext()
        {
            switch (_state)
            {
                case 1:
                    _enumerator = _source.GetEnumerator();
                    _state = 2;
                    goto case 2;
                case 2:
                    System.Diagnostics.Debug.Assert(_enumerator != null);
                    while (_enumerator.MoveNext())
                    {
                        TSource item = _enumerator.Current;
                        if (_predicate(item))
                        {
                            _current = _selector(item);
                            return true;
                        }
                    }

                    Dispose();
                    break;
            }

            return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TResult> WhereSelect_2<TSource, TResult>(this IEnumerable<TSource> source,
        Func<TSource, bool> predicate, Func<TSource, TResult> selector)
    {
        return new WhereSelectEnumerableIterator<TSource, TResult>(source, predicate, selector);
    }
}
