using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using LinqBenchmark.Benchmark;

namespace LinqBenchmark
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(WhereSelectBenchmark).Assembly).Run(args);

            //var customConfig = ManualConfig
            //    .Create(DefaultConfig.Instance)
            //    .AddValidator(JitOptimizationsValidator.FailOnError)
            //    .AddDiagnoser(MemoryDiagnoser.Default)
            //    .AddColumn(StatisticColumn.AllStatistics)
            //    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core50))
            //    .AddExporter(DefaultExporters.Markdown);

            //BenchmarkRunner.Run<WhereSelectBenchmark>(customConfig);
        }
    }
}
