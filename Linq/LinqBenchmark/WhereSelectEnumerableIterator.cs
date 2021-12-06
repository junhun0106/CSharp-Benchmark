using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LinqBenchmark
{
    // https://source.dot.net/#System.Linq/System/Linq/Where.cs,0e5ab1a57b7e1438
    public static partial class EnumerableExtensions
    {
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
}
