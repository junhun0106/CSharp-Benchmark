using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace StringBuilderBenchMarker
{
    public static class StringBuilderPool
    {
        private static readonly ObjectPool<StringBuilder> _shared = new DefaultObjectPoolProvider().CreateStringBuilderPool();

        public static StringBuilder Get() => _shared.Get();

        public static void Return(StringBuilder sb)
        {
            if (sb != null) {
                _shared.Return(sb);
            }
        }
    }
}
