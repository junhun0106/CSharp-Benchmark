using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmark;

[Description("struct.Equal 비교")]
public class StructEqualBenchmark : BenchmarkBase, IValidate
{
    private readonly TestStruct _comparer = new(1, EType.B, new DateTime(2021, 11, 30), "1");
    private readonly TestStruct_NotImpl _comparer_notImpl = new(1, EType.B, new DateTime(2021, 11, 30), "1");

    [Benchmark]
    public void Equals_PureStruct()
    {
        var comparer_notImpl = new TestStruct_NotImpl(1, EType.B, new DateTime(2021, 11, 30), "1");
        _ = comparer_notImpl.HasValue();
    }

    [Benchmark]
    public void Equals_EquatableImplement()
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

[Description("값 복사 vs in 비교")]
public class StructValueCopyVsIn : BenchmarkBase
{
    /*
|    Method |     Mean |    Error |   StdDev | Allocated |
|---------- |---------:|---------:|---------:|----------:|
| ValueCopy | 27.22 ns | 0.550 ns | 0.612 ns |         - |
|        In | 26.54 ns | 0.534 ns | 0.694 ns |         - |    
    
|    Method |     Mean |    Error |   StdDev | Allocated |
|---------- |---------:|---------:|---------:|----------:|
| ValueCopy | 26.84 ns | 0.425 ns | 0.332 ns |         - |
|        In | 27.03 ns | 0.540 ns | 0.505 ns |         - |
    */
    public readonly record struct TestStruct
    {
        public readonly int Value;

        public TestStruct(int value)
        {
            this.Value = value;
        }
    }

    public static TestStruct @struct = new(1);

    [Benchmark]
    public void ValueCopy()
    {
        for (int i = 0; i < 100; ++i)
        {
            _ValueCopy(@struct);
        }
    }

    public int _ValueCopy(TestStruct value)
    {
       return value.Value + 1;
    }

    [Benchmark]
    public void In()
    {
        for (int i = 0; i < 100; ++i)
        {
            _In(in @struct);
        }
    }

    public int _In(in TestStruct value)
    {
        return value.Value + 1;
    }

    [Benchmark]
    public void Ref()
    {
        for (int i = 0; i < 100; ++i)
        {
            _Ref(ref @struct);
        }
    }

    public int _Ref(ref TestStruct value)
    {
        return value.Value + 1;
    }
}