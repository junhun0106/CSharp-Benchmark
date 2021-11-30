using System;
using System.Collections.Generic;
using System.Linq;
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
    public class DeletgateBenchmark
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

    internal static class Program
    {
        private static void Main(string[] args)
        {
            //BenchmarkSwitcher.FromAssembly(typeof(DeletgateBenchmark).Assembly).Run(args);

            var customConfig = ManualConfig
                .Create(DefaultConfig.Instance)
                .AddValidator(JitOptimizationsValidator.FailOnError)
                .AddDiagnoser(MemoryDiagnoser.Default)
                .AddColumn(StatisticColumn.AllStatistics)
                .AddJob(Job.Default.WithRuntime(CoreRuntime.Core50))
                .AddExporter(DefaultExporters.Markdown);

            BenchmarkRunner.Run<MSDNBenchmark>(customConfig);
        }
    }
}
