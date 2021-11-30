using System;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Validators;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;

using Tester.Benchmark;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

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
            //BenchmarkSwitcher.FromAssembly(typeof(SelectToListBenchmark).Assembly).Run(args);

            var customConfig = ManualConfig
              .Create(DefaultConfig.Instance)
              .AddValidator(JitOptimizationsValidator.FailOnError)
              .AddDiagnoser(MemoryDiagnoser.Default)
              .AddColumn(StatisticColumn.AllStatistics)
              //.AddJob(Job.Default.WithRuntime(ClrRuntime.Net48))
              //.AddJob(Job.Default.WithRuntime(CoreRuntime.Core31))
              .AddJob(Job.Default.WithRuntime(CoreRuntime.Core50))
              .AddExporter(DefaultExporters.Markdown);

            BenchmarkRunner.Run<CloneBenchmark>(customConfig);
        }
    }
}
