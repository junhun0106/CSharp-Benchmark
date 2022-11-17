using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmark
{
    public class TryCatchWhenBenchmark : BenchmarkBase
    {
        /*
         사소한 차이 이므로, 가독성이 좋은 코드를 선택하면 되겠다. 

         |       Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
         |------------- |---------:|---------:|---------:|-------:|----------:|
         |   TryCatchIf | 34.03 us | 0.135 us | 0.113 us | 0.0610 |      2 KB |
         | TryCatchWhen | 34.73 us | 0.223 us | 0.209 us | 0.0610 |      2 KB |
         */
        const bool @throw = false;
        
        class BecnmarkThrowException : Exception { }

        [Benchmark]
        public void TryCatchIf()
        {
            for (int i = 0; i < 10; ++i)
            {
                try
                {
                    throw new BecnmarkThrowException();
                }
                catch (Exception)
                {
                    if (@throw)
                    {
                        throw;
                    }
                    else
                    {
                        //
                    }
                }
            }
        }

        [Benchmark]
        public void TryCatchWhen()
        {
            for (int i = 0; i < 10; ++i)
            {
                try
                {
                    throw new BecnmarkThrowException();
                }
                catch (Exception) when (!@throw)
                {

                }
            }
        }

        static bool IsThrow() => @throw;
    }
}
