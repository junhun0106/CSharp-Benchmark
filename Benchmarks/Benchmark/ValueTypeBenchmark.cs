using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmark;

/*
    https://adamsitnik.com/Value-Types-vs-Reference-Types/
    위 테스트들이 실제인지 테스트해보자
 */

public class ValueTypeInvokingInterfaceMethodSmart : BenchmarkBase
{
    /*
|         Method |      Mean |     Error |    StdDev | Ratio |  Gen 0 | Allocated |
|--------------- |----------:|----------:|----------:|------:|-------:|----------:|
|      ValueType | 0.3777 ns | 0.0048 ns | 0.0040 ns | 1.000 | 0.0001 |       2 B |
| ValueTypeSmart | 0.0003 ns | 0.0002 ns | 0.0002 ns | 0.001 |      - |         - |
    */

    interface IInterface { void Func(); }
    struct StructWithInterface : IInterface { public void Func() { } }

    private StructWithInterface value = new StructWithInterface();

    [Benchmark(Baseline = true, OperationsPerInvoke = 16)]
    public void ValueType()
    {
        // case 1.
        static void Invoke(IInterface value) { value.Func(); }
        Invoke(value);
    }

    [Benchmark(OperationsPerInvoke = 16)]
    public void ValueTypeSmart()
    {
        static void Invoke<T>(T value) where T : IInterface { value.Func(); }
        Invoke(value);
    }
}
