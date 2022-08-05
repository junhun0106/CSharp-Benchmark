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