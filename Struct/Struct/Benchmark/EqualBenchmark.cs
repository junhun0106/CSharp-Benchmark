using BenchmarkDotNet.Attributes;
using System;
using System.Runtime.CompilerServices;

namespace Struct.Benchmark
{
    [MemoryDiagnoser]
    [BenchmarkDescription("struct.Equal 비교")]
    public class EqualBenchmark : IValidate
    {
        private readonly TestStruct _comparer = new(1, EType.B, new DateTime(2021, 11, 30), "1");
        private readonly TestStruct_NotImpl _comparer_notImpl = new(1, EType.B, new DateTime(2021, 11, 30), "1");

        [Benchmark]
        public void NotImpl()
        {
            var comparer_notImpl = new TestStruct_NotImpl(1, EType.B, new DateTime(2021, 11, 30), "1");
            _ = comparer_notImpl.HasValue();
        }

        [Benchmark]
        public void EquatableImpl()
        {
            var comparer = new TestStruct(1, EType.B, new DateTime(2021, 11, 30), "1");
            _ = comparer.HasValue();
        }

        public void Validate()
        {
            const string exception = "EqualBenchmark.Invalidate";

            var comparer = new TestStruct(1, EType.B, new DateTime(2021, 11, 30), "1");
            var comparer_notImpl = new TestStruct_NotImpl(1, EType.B, new DateTime(2021, 11, 30), "1");

            var @default = default(TestStruct);
            var default_notImpl = default(TestStruct_NotImpl);

            if (!_comparer.Equals(comparer))
            {
                throw new Exception(exception);
            }

            if (!comparer_notImpl.Equals(_comparer_notImpl))
            {
                throw new Exception(exception);
            }

            if (!comparer.HasValue())
            {
                throw new Exception(exception);
            }

            if (!@default.IsNull())
            {
                throw new Exception(exception);
            }

            if (!comparer_notImpl.HasValue())
            {
                throw new Exception(exception);
            }

            if (!default_notImpl.IsNull_2())
            {
                throw new Exception(exception);
            }
        }
    }

    public static class TestStructExtensions
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
}
