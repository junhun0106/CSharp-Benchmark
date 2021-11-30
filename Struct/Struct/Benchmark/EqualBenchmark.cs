using BenchmarkDotNet.Attributes;
using System;

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
            if(_comparer_notImpl.Equals(comparer_notImpl))
            {
                //
            }
        }

        [Benchmark]
        public void EquatableImpl()
        {
            var comparer = new TestStruct(1, EType.B, new DateTime(2021, 11, 30), "1");
            if (_comparer.Equals(comparer))
            {
                //
            }
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

            //무조건 실패
            //if (!default_notImpl.IsNull())
            //{
            //    throw new Exception(exception);
            //}
        }
    }

    public static class TestStructExtensions
    {
        public static bool HasValue<T>(this T self) where T : struct, ITestStruct
        {
            var e = self.Equals(default);
            return e == false;
        }

        public static bool IsNull<T>(this T self) where T : struct, ITestStruct
        {
            var e = self.HasValue();
            return e == false;
        }
    }
}
