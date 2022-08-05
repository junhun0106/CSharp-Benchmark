using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmark;

public class EnumNameOfTest : BenchmarkBase
{
    enum ETest
    {
        Test,
    }

    [Benchmark]
    public void ToString_()
    {
        _ = ETest.Test.ToString();
    }

    [Benchmark]
    public void NameOf()
    {
        _ = nameof(ETest.Test);
    }
}
