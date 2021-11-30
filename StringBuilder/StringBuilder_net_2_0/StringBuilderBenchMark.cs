using System.Text;
using BenchmarkDotNet.Attributes;
using Cysharp.Text;

namespace StringBuilderBenchMarker
{
    public static class StringBuilderBenchMark
    {
        private const string testString = "1234567890";

        [MemoryDiagnoser]
        public class StringBuilderAppendBenchMark
        {
            [Benchmark]
            public void StringBuilderAppend()
            {
                var sb = new StringBuilder();
                sb.Append(testString);
                sb.Append(testString);
                sb.Append(testString);
                sb.Append(testString);
                sb.Append(testString);
            }

            [Benchmark]
            public void StringBuilderPoolAppend()
            {
                var sb = StringBuilderPool.Get();
                try {
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                } finally {
                    StringBuilderPool.Return(sb);
                }
            }

            [Benchmark]
            public void ZStringAppend()
            {
                using (var sb = ZString.CreateStringBuilder()) {
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                }
            }

            [Benchmark]
            public void ValueStringBuilderAppend()
            {
                var sb = new ValueStringBuilder();
                try {
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                } finally {
                    sb.Dispose();
                }
            }
        }

        [MemoryDiagnoser]
        public class StringBuilderAppendWithCapacityBenchMark
        {
            private string testString;

            [GlobalSetup]
            public void Init()
            {
                const int count = 1000;
                for (int i = 0; i < count; ++i) {
                    testString += i.ToString();
                }
            }

            [Benchmark]
            public void StringBuilderAppend()
            {
                var sb = new StringBuilder(testString.Length * 5);
                sb.Append(testString);
                sb.Append(testString);
                sb.Append(testString);
                sb.Append(testString);
                sb.Append(testString);
            }

            [Benchmark]
            public void StringBuilderPoolAppend()
            {
                // 이미 내부에서 Capacity 100으로 지정
                var sb = StringBuilderPool.Get();
                try {
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                } finally {
                    StringBuilderPool.Return(sb);
                }
            }

            [Benchmark]
            public void ZStringAppend()
            {
                using (var sb = ZString.CreateStringBuilder()) {
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                }
            }

            [Benchmark]
            public void ValueStringBuilderAppend()
            {
                var sb = new ValueStringBuilder(testString.Length * 5);
                try {
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                    sb.Append(testString);
                } finally {
                    sb.Dispose();
                }
            }
        }
    }
}
