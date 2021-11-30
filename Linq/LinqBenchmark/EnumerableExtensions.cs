using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LinqBenchmark
{
    public static class EnumerableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TResult> WhereSelect<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, bool> predicate, Func<TSource, TResult> selector)
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
    }
}
