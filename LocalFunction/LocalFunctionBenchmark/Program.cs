using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using Microsoft.CodeAnalysis.CSharp;

namespace LocalFunctionBenchmark
{
    /// <summary>
    /// https://docs.microsoft.com/ko-kr/dotnet/csharp/programming-guide/classes-and-structs/local-functions#heap-allocations
    /// 로컬 함수 힙 할당 예시 테스트
    /// </summary>
    [MemoryDiagnoser]
    public class MSDNBenchmark
    {
        [Benchmark]
        public async Task Lambda()
        {
            await PerformLongRunningWorkLambda("addess", 0, "index");
        }

        [Benchmark]
        public async Task LocalFunction()
        {
            await PerformLongRunningWork("addess", 0, "index");
        }

        private async Task<string> PerformLongRunningWorkLambda(string address, int index, string name)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException(message: "An address is required", paramName: nameof(address));
            if (index < 0)
                throw new ArgumentOutOfRangeException(paramName: nameof(index), message: "The index must be non-negative");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(message: "You must supply a name", paramName: nameof(name));

            Func<Task<string>> longRunningWorkImplementation = async () =>
            {
                var interimResult = await FirstWork(address);
                var secondResult = await SecondStep(index, name);
                return $"The results are {interimResult} and {secondResult}. Enjoy.";
            };

            return await Work(longRunningWorkImplementation);
        }

        private async Task<string> PerformLongRunningWork(string address, int index, string name)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException(message: "An address is required", paramName: nameof(address));
            if (index < 0)
                throw new ArgumentOutOfRangeException(paramName: nameof(index), message: "The index must be non-negative");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(message: "You must supply a name", paramName: nameof(name));

            async Task<string> longRunningWorkImplementation()
            {
                var interimResult = await FirstWork(address);
                var secondResult = await SecondStep(index, name);
                return $"The results are {interimResult} and {secondResult}. Enjoy.";
            }

            return await Work(longRunningWorkImplementation);
        }

        private Task<string> Work(Func<Task<string>> func)
        {
            return func();
        }

        private async Task<int> FirstWork(string address)
        {
            return 0;
        }

        private async Task<int> SecondStep(int index, string name)
        {
            return 0;
        }
    }

    [MemoryDiagnoser]
    public class DelegateParameterBenchmark
    {
        private struct A
        {
            public readonly int Index;
            public readonly string @string;

            public A(int index, string @string)
            {
                Index = index;
                this.@string = @string;
            }
        }

        private const int _capacity = 1000;
        private readonly List<A> _list = new(_capacity);

        [GlobalSetup]
        public void GlobalSetUp()
        {
            for (int i = 0; i < _capacity; ++i)
            {
                var index = (i % 10);
                _list.Add(new A(index, index.ToString()));
            }
        }

        [Benchmark]
        public void Func()
        {
            var _comparer = new A(0, "0");
            _ = _list.Where(x => x.Index == _comparer.Index).Select(x => new A(_comparer.Index, _comparer.Index.ToString()));
        }

        [Benchmark]
        public void LocalFunc()
        {
            var _comparer = new A(0, "0");
            bool where(A x) => x.Index == _comparer.Index;
            A select(A x) => new A(_comparer.Index, _comparer.Index.ToString());
            _ = _list.Where(x => where(x)).Select(x => select(x));
        }

        // 파라미터를 넣은 경우가 더 느린 것 확인하여 더 이상 테스트 할 필요 없어서 주석
        //[Benchmark]
        //public void LocalFuncInParameter()
        //{
        //    bool where(A x) => x.@string == "0";
        //    string select(A x) => x.@string; 
        //    _ = _list.Where(where).Select(select);
        //}

        // static로 변환이 가능한 func이라면, 모두가 같은 할당을 보인다
        // 즉, static로 선언한 경우는 힙 할당을 피하겠다는 의미.
        // 더 이상 테스트 할 필요가 없으므로 제외.
        //[Benchmark]
        //public void StaticLocalFunc()
        //{
        //    static bool where(A x) => x.@string == "0"; 
        //    static string select(A x) => x.@string;
        //    _ = _list.Where(x => where(x)).Select(x => select(x));
        //}

        // 파라미터를 넣은 경우가 더 느린 것 확인하여 더 이상 테스트 할 필요 없어서 주석
        //[Benchmark]
        //public void StaticLocalFuncInParameter()
        //{
        //    static bool where(A x) => x.@string == "0";
        //    static string select(A x) => x.@string;
        //    _ = _list.Where(where).Select(select);
        //}

        // static 함수와 static local 함수가 다른 점이 없음 확을 확인하여 주석
        //[Benchmark]
        //public void StaticFunc()
        //{
        //    _ = _list.Where(x => g_where(x)).Select(x => g_select(x));
        //}

        // 파라미터를 넣은 경우가 더 느린 것 확인하여 더 이상 테스트 할 필요 없어서 주석
        //[Benchmark]
        //public void StaticFuncInParameter()
        //{
        //    _ = _list.Where(g_where).Select(g_select);
        //}

        //static bool g_where(A x) => x.@string == "0";
        //static string g_select(A x) => x.@string;
    }

    [MemoryDiagnoser]
    public class DelegateIsBenchmark
    {
        private Delegate actionDelegate;
        private Delegate funcDelegate;

        [GlobalSetup]
        public void GlobalSetUp()
        {
            actionDelegate = (Action)(() => { });
            funcDelegate = (Func<bool>)(() => true);
        }

        [Benchmark]
        public void IsAction()
        {
            if(actionDelegate is Action action)
            {
                action();
            }
        }

        [Benchmark]
        public void IsFunc()
        {
            if (funcDelegate is Func<bool> func)
            {
                _ = func();
            }
        }
    }

    [MemoryDiagnoser]
    public class LocalFuncVsDelegae
    {
        [Benchmark]
        public void LocalFunc_1()
        {
            int sum = 0;
            void test()
            {
                for (int i = 0; i < 100; i++)
                {
                    sum += i;
                }
            }

            test();

            sum += sum;
        }

        [Benchmark]
        public void LocalFunc_2()
        {
            int sum = 0;
            static int test(ref int s)
            {
                for (int i = 0; i < 100; i++)
                {
                    s += i;
                }

                return s;
            }

            test(ref sum);

            sum += sum;
        }

        [Benchmark]
        public void Action()
        {
            int sum = 0;
            Action action = () =>
            {
                for (int i = 0; i < 100; i++)
                {
                    sum += i;
                }
            };

            action();

            sum += sum;
        }
    }

    [MemoryDiagnoser]
    public class InterpolatedStringHandlerTest
    {
        //public static class Log
        //{
        //    public static void Debug(string message)
        //    {

        //    }
        //}

        //public static class Log_2
        //{
        //    //[InterpolatedStringHandler]
        //    public ref struct DebugLoggerStringHandler
        //    {
        //        private DefaultInterpolatedStringHandler builder;
        //        public DebugLoggerStringHandler(int literalLength, int formattedCount)
        //        {
        //            this.builder = new(literalLength, formattedCount);
        //        }

        //        public void AppendLiteral(string value)
        //            => this.builder.AppendLiteral(value);
        //        public void AppendFormatted<T>(T t, int alignment = 0, string format = null)
        //            => this.builder.AppendFormatted(t, alignment, format);
        //        public void AppendFormatted(DateTime datetime)
        //            => this.builder.AppendFormatted(datetime, format: "u");
        //        public void AppendFormatted(bool boolean)
        //            => this.builder.AppendLiteral(boolean ? "TRUE" : "FALSE");
        //        public string ToStringAndClear()
        //            => this.builder.ToStringAndClear();
        //    }

        //    public static void Debug(string message)
        //    {

        //    }

        //    public static void Debug(ref DebugLoggerStringHandler handler)
        //    {
        //        _ = handler.ToStringAndClear();
        //    }
        //}

        //const string 보간 = "보간";

        //[Benchmark]
        //public void 단순문자열()
        //{
        //    Log.Debug("단순문자열");
        //}

        //[Benchmark]
        //public void 보간문자열()
        //{
        //    Log.Debug($"{보간}문자열");
        //}

        //[Benchmark]
        //public void 단순문자열WithInterpolatedStringHandler()
        //{
        //    Log_2.Debug("단순문자열");
        //}

        //[Benchmark]
        //public void 보간문자열WithInterpolatedStringHandler()
        //{
        //    Log_2.Debug($"{보간}문자열");
        //}
    }

    [MemoryDiagnoser]
    public class StringInternTest
    {
        public static class Log
        {
            public static string Debug(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int lineNumber = -1)
            {
                var mn = memberName;
                var sf = sourceFilePath;

                return String.Empty;
            }

            public static string Debug_2(string message)
            {
                return String.Empty;
            }
        }

        [Benchmark]
        public void Default()
        {
            Log.Debug("로그 메시지");
        }

        [Benchmark]
        public void StringIntern()
        {
            Log.Debug_2("로그 메시지");
        }
    }

    [MemoryDiagnoser]
    public class SubStringTest
    {
        /*
         * test1
|        Method |     Mean |     Error |    StdDev |    StdErr |      Min |       Q1 |   Median |       Q3 |      Max |          Op/s |  Gen 0 | Allocated |
|-------------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|--------------:|-------:|----------:|
| RangeOperator | 6.419 ns | 0.1212 ns | 0.1699 ns | 0.0327 ns | 6.105 ns | 6.311 ns | 6.413 ns | 6.528 ns | 6.813 ns | 155,790,742.6 | 0.0033 |      56 B |
|     SubString | 6.551 ns | 0.1484 ns | 0.1709 ns | 0.0382 ns | 6.310 ns | 6.410 ns | 6.547 ns | 6.680 ns | 6.863 ns | 152,659,269.2 | 0.0033 |      56 B |
         
         * test2
         
         */
        string sentence = "the quick brown fox";

        [Benchmark]
        public void RangeOperator()
        {
            var sub = sentence[0..^4];
        }

        [Benchmark]
        public void SubString()
        {
            var sub = sentence.Substring(0, sentence.Length - 4);
        }
    }

    [MemoryDiagnoser]
    public class ArrayCopyTest
    {
        const int size = sizeof(int);
        const int on = 1;
        const int keepAliveInterval = 10000;   // Send a packet once every 10 seconds.
        const int retryInterval = 1000;        // If no response, resend every second.

        [Benchmark]
        public void ArrayCopy()
        {
            byte[] inArray = new byte[size * 3];
            Array.Copy(BitConverter.GetBytes(on), 0, inArray, 0, size);
            Array.Copy(BitConverter.GetBytes(keepAliveInterval), 0, inArray, size, size);
            Array.Copy(BitConverter.GetBytes(retryInterval), 0, inArray, size * 2, size);
        }

        [Benchmark]
        public void BlockCopy()
        {
            byte[] inArray = new byte[size * 3];
            Buffer.BlockCopy(BitConverter.GetBytes(on), 0, inArray, 0, size);
            Buffer.BlockCopy(BitConverter.GetBytes(keepAliveInterval), 0, inArray, size, size);
            Buffer.BlockCopy(BitConverter.GetBytes(retryInterval), 0, inArray, size * 2, size);
        }

        static readonly byte[] onBytes = BitConverter.GetBytes(1);
        static readonly byte[] keepAliveIntervalBytes = BitConverter.GetBytes(1000);
        static readonly byte[] retryIntervalBytes = BitConverter.GetBytes(1000);

        [Benchmark]
        public void ArrayCopyWithStatic()
        {
            const int size = sizeof(int);
            byte[] inArray = new byte[size * 3];
            Array.Copy(onBytes, 0, inArray, 0, size);
            Array.Copy(keepAliveIntervalBytes, 0, inArray, size, size);
            Array.Copy(retryIntervalBytes, 0, inArray, size * 2, size);
        }

        [Benchmark]
        public void BlockCopyWithStatic()
        {
            const int size = sizeof(int);
            byte[] inArray = new byte[size * 3];
            Buffer.BlockCopy(onBytes, 0, inArray, 0, size);
            Buffer.BlockCopy(keepAliveIntervalBytes, 0, inArray, size, size);
            Buffer.BlockCopy(retryIntervalBytes, 0, inArray, size * 2, size);
        }
    }

    [MemoryDiagnoser]
    public class NameOfTest
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

    [MemoryDiagnoser]
    public class AddRangeTest
    {
        class Data
        {
            public int A;
            public int B;
        }

        private static List<Data> list = new List<Data>
        {
            new Data(),
            new Data(),
            new Data(),
            new Data(),
        };

        [Benchmark]
        public void Add()
        {
            Add(list);
        }

        [Benchmark]
        public void AddRange()
        {
            AddRange(list);
        }

        [Benchmark]
        public void AddRange2()
        {
            AddRange2(list);
        }

        private void Add(IEnumerable<Data> list)
        {
            var l = new List<Data>();
            foreach(var item in list) l.Add(item);
        }

        void AddRange(IEnumerable<Data> list)
        {
            var l = new List<Data>();
            l.AddRange(list);
        }

        void AddRange2(IReadOnlyList<Data> list)
        {
            var l = new List<Data>();
            l.Capacity = list.Count;
            l.AddRange(list);
        }
    }

    internal static class Program
    {
        private static void Main(string[] args)
        {
            //int moduler = 1;
            //var list = new List<string>();

            //Console.WriteLine(list[moduler % list.Count]);

            //BenchmarkSwitcher.FromAssembly(typeof(DeletgateBenchmark).Assembly).Run(args);

            var customConfig = ManualConfig
                .Create(DefaultConfig.Instance)
                .AddValidator(JitOptimizationsValidator.FailOnError)
                .AddDiagnoser(MemoryDiagnoser.Default)
                .AddColumn(StatisticColumn.AllStatistics)
                .AddJob(Job.Default.WithRuntime(CoreRuntime.Core60))
                .AddExporter(DefaultExporters.Markdown);

            BenchmarkRunner.Run<AddRangeTest>(customConfig);
        }
    }
}
