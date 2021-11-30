using System;
using System.Linq;
using System.Reflection;
using System.Text;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

namespace Struct
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => type.GetCustomAttribute<BenchmarkDescriptionAttribute>() != null)
                .ToList();

            var sb = new StringBuilder();
            for (int index = 0; index < types.Count; index++)
            {
                var type = types[index];
                // validate
                {
                    var instance = Activator.CreateInstance(type);
                    if(instance is IValidate validate)
                    {
                        validate.Validate();
                    }
                    else
                    {
                        var previousColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{type.Name} not impl IValidate");
                        Console.ForegroundColor = previousColor;
                    }
                }
                var attr = type.GetCustomAttribute<BenchmarkDescriptionAttribute>();
                sb.AppendLine($"#[{index}]\t{type.Name} - {attr.Description}");
            }
            Console.WriteLine("select index, if you want exit. input \"e\" or \"E\"");
            Console.WriteLine(sb.ToString());
            Type selectType = null;
            while (true) {
                var selectIndexString = Console.ReadLine();

                if("e".Equals(selectIndexString, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (int.TryParse(selectIndexString, out var selectIndex))
                {
                    if(types.Count > selectIndex)
                    {
                        selectType = types[selectIndex];
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"out of range, {types.Count} <= {selectIndex}, try again...");
                    }
                }
                else
                {
                    Console.WriteLine($"not found index[{selectIndexString}], try again...");
                }
            }

            if (selectType != null)
            {
                var customConfig = ManualConfig
                    .Create(DefaultConfig.Instance)
                    .AddValidator(JitOptimizationsValidator.FailOnError)
                    .AddDiagnoser(MemoryDiagnoser.Default)
                    .AddColumn(StatisticColumn.AllStatistics)
                    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core50))
                    .AddExporter(DefaultExporters.Markdown);

                BenchmarkRunner.Run(selectType, customConfig);
            }
        }
    }
}
