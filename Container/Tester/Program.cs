using System;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Reports;

using Tester.Benchmark;

namespace Tester
{
    [MemoryDiagnoser]
    public class IsBanchmark
    {
        private IFieldMap test = new A();

        public interface IFieldMap
        {

        }

        public interface IDungeonMap : IFieldMap
        {

        }

        public class A : IFieldMap, IDungeonMap
        {

        }

        [Benchmark]
        public void Is()
        {
            if (test is IDungeonMap)
            {

            }
        }

        [Benchmark]
        public void TryGet()
        {
            if (TryGetA(test, out var b))
            {

            }
        }

        private bool TryGetA(IFieldMap a, out IDungeonMap b)
        {
            if (a is IDungeonMap dm)
            {
                b = dm;
                return true;
            }

            b = null;
            return false;
        }
    }

    [MemoryDiagnoser]
    public class AggresiveInlineBenchmark
    {
        [Benchmark]
        public void Sum()
        {
            for (int i = 0; i < 100; ++i) Sum(i, i);
        }

        [Benchmark]
        public void SumInline()
        {
            for (int i = 0; i < 100; ++i) SumInline(i, i);
        }

        private static int Sum(int a, int b) => a + b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SumInline(int a, int b) => a + b;
    }

    [MemoryDiagnoser]
    public class SealedAttributeBenchmark
    {
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public class UnsealedAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public sealed class SealedAttribute : Attribute { }

        [Sealed]
        [Unsealed]
        public class A { }

        private readonly A _a = new();

        [Benchmark]
        public void Sealed()
        {
            _ = _a.GetType().GetCustomAttribute<SealedAttribute>();
        }

        [Benchmark]
        public void SealedWithInherit()
        {
            _ = _a.GetType().GetCustomAttribute<SealedAttribute>(inherit: false);
        }

        [Benchmark]
        public void Unsealed()
        {
            _ = _a.GetType().GetCustomAttribute<UnsealedAttribute>();
        }

        [Benchmark]
        public void UnsealedWithInherit()
        {
            _ = _a.GetType().GetCustomAttribute<UnsealedAttribute>(inherit: false);
        }
    }

    [MemoryDiagnoser]
    public class ArrayOrderByBenchmark
    {
        public class TestClass
        {
            public readonly int Index;

            public TestClass(int index)
            {
                Index = index;
            }
        }

        private class AComparer : IComparer<TestClass>
        {
            public int Compare(TestClass x, TestClass y)
            {
                return x.Index.CompareTo(y.Index);
            }
        }

        private static readonly AComparer g_Comparer = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TestClass[] CreateArray()
        {
            const int count = 100;
            var array = new TestClass[count];
            for (int index = 0; index < count; ++index)
            {
                array[index] = new TestClass(count - index - 1);
            }
            return array;
        }

        [Benchmark]
        public void OrderBy()
        {
            var array = CreateArray();
            array = array.OrderBy(x => x.Index).ToArray();
        }

        [Benchmark]
        public void Sort()
        {
            var array = CreateArray();
            Array.Sort(array, g_Comparer);
        }
    }

    [MemoryDiagnoser]
    public class CloneBenchmark
    {
        private byte[] _src;

        [GlobalSetup]
        public void GlobalSetUp()
        {
            const int length = 4096;
            _src = new byte[length];
            foreach (var i in Enumerable.Range(0, length))
            {
                _src[i] = (byte)i;
            }
        }

        [Benchmark]
        public Memory<byte> Span()
        {
            Span<byte> dest = stackalloc byte[_src.Length];
            Span<byte> src = _src;
            src.CopyTo(dest);
            return dest.ToArray();
        }

        [Benchmark]
        public Memory<byte> MemberwiseClone()
        {
            return (byte[])_src.Clone();
        }

        [Benchmark]
        public unsafe Memory<byte> UnsafeMemoryCopy()
        {
            var count = _src.Length;
            var dest = new byte[count];
            fixed (byte* srcPointer = &this._src[0])
            {
                fixed(byte* destPointer = dest)
                {
                    Buffer.MemoryCopy(srcPointer, destPointer, count, count);
                }
            }

            return dest;
        }
    }

    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("1. select bechmark");
            Console.WriteLine("2. write all bechmark");
            Console.WriteLine("'e' or 'E' exit");
            while (true)
            {
                var line = Console.ReadLine();
                if (line.Equals("e", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (int.TryParse(line, out var num))
                {
                    if (num == 1)
                    {
                        Selecter();
                    }
                    else if (num == 2)
                    {
                        Writer(args);
                    }
                }
            }
        }

        static void Selecter()
        {
            Console.WriteLine("press any key...");
            Console.ReadLine();
            Console.WriteLine("start...");

            var benchmark = new DictionaryValuesBenchmark();
            benchmark.GlobalSetUp();
            const int testCount = 100000;
            for (int i = 0; i < testCount; ++i)
            {
                benchmark.ValueWhere();
            }

            for (int i = 0; i < testCount; ++i)
            {
                benchmark.Value2Where();
            }

            for (int i = 0; i < testCount; ++i)
            {
                benchmark.ForeachWhere();
            }

            Console.WriteLine("finish...");
            while (true)
            {
                Console.ReadLine();
                break;
            }
        }

        static void Writer(string[] args)
        {
            foreach (var summary in BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args))
            {
                WriteSummary(summary);
                break;
            }
        }

        static void WriteSummary(Summary summary)
        {
            var solutionDir = GetSolutionDirectory();
            if (solutionDir is null)
            {
                return;
            }

            var targetType = GetTargetType(summary);
            if (targetType is null)
            {
                return;
            }

            var title = targetType.Name;
            var pointIndex = targetType.Namespace.IndexOf('.');
            if (pointIndex >= 0)
                title = $"{EndSubstring(targetType.Namespace, pointIndex + 1)}.{targetType.Name}";

            var resultsPath = Path.Combine(solutionDir, "Results");
            _ = Directory.CreateDirectory(resultsPath);

            var filePath = Path.Combine(resultsPath, $"{title}.md");

            if (File.Exists(filePath))
                File.Delete(filePath);

            using var fileWriter = new StreamWriter(filePath, false, Encoding.UTF8);
            var logger = new StreamLogger(fileWriter);

            logger.WriteLine($"## {title}");
            logger.WriteLine();

            logger.WriteLine("### Source");
            var sourceLink = new StringBuilder("../LinqBenchmarks");
            foreach (var folder in targetType.Namespace.Split('.').AsEnumerable().Skip(1))
                _ = sourceLink.Append($"/{folder}");
            _ = sourceLink.Append($"/{targetType.Name}.cs");
            logger.WriteLine($"[{targetType.Name}.cs]({sourceLink})");
            logger.WriteLine();

            logger.WriteLine("### Results:");

            MarkdownExporter.GitHub.ExportToLog(summary, logger);
        }

        static string EndSubstring(string str, int index)
            => str.Substring(index, str.Length - index);

        static Type GetTargetType(Summary summary)
        {
            var targetTypes = summary.BenchmarksCases.Select(i => i.Descriptor.Type).Distinct().ToList();
            return targetTypes.Count == 1 ? targetTypes[0] : null;
        }

        static string GetSolutionDirectory()
        {
            var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            while (!string.IsNullOrEmpty(dir))
            {
                if (Directory.EnumerateFiles(dir, "*.sln", SearchOption.TopDirectoryOnly).Any())
                    return dir;

                dir = Path.GetDirectoryName(dir);
            }

            return null;
        }
    }
}
