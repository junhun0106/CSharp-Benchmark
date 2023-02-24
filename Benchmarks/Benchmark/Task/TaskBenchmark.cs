using System;
using System.Runtime.CompilerServices;
using System.Threading;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmark.Task
{
    public class TaskBenchmark_WhenAllVsForEachAsync : BenchmarkBase
    {
        const int count = 10;
        static readonly string[] names = new string[count];
        static readonly ParallelOptions options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount * 2,
        };
        static HttpClient httpClient;
        static SemaphoreSlim semaphoreSlim;

        [GlobalSetup]
        public void Setup()
        {
            semaphoreSlim = new SemaphoreSlim(count);

            var handler = new HttpClientHandler
            {
                MaxConnectionsPerServer = count,
            };

            httpClient = new HttpClient(handler)
            {
                BaseAddress = new("https://www.google.com")
            };
            httpClient.DefaultRequestHeaders.Connection.Clear();
            httpClient.DefaultRequestHeaders.Connection.Add("Keep-Alive");
            httpClient.DefaultRequestHeaders.ConnectionClose = false;

            for (int i = 0; i < count; i++)
            {
                names[i] = $"{i}.txt";
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static async Task WriteFileAsync(string file, string content)
        {
            /*
|               Method |     Mean |   Error |  StdDev | Allocated |
|--------------------- |---------:|--------:|--------:|----------:|
|            TaskArray | 252.5 ms | 5.04 ms | 5.17 ms |   1.05 MB |
| ParallelForeachAsync | 239.2 ms | 4.73 ms | 7.51 ms |   1.15 MB |
             */

            await File.WriteAllTextAsync(file, content);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static async Task ReadHttpAsync(string caller, string name)
        {
            /*
|               Method |     Mean |     Error |    StdDev |    Gen0 |   Gen1 | Allocated |
|--------------------- |---------:|----------:|----------:|--------:|-------:|----------:|
|            TaskArray | 6.012 ms | 0.0172 ms | 0.0160 ms | 54.6875 |      - | 937.63 KB |
| ParallelForeachAsync | 1.664 ms | 0.0212 ms | 0.0188 ms | 56.6406 | 1.9531 | 922.29 KB |
             */
            try
            {
                await semaphoreSlim.WaitAsync();
                var response = await httpClient.GetAsync("").ConfigureAwait(false);
                //Console.WriteLine($"[{caller} | {name} | {Environment.CurrentManagedThreadId}] {response.StatusCode}");
            }
            catch
            {
                // ignore
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        [Benchmark]
        public async Task TaskArray()
        {
            var tasks = new Task[count];
            for (int i = 0; i < count; ++i)
            {
                var name = names[i];
                tasks[i] = ReadHttpAsync(nameof(TaskArray), name);
                //tasks[i] = WriteFileAsync(name, name);
            }

            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task ParallelForeachAsync()
        {
            await Parallel.ForEachAsync(names, options, async (name, _) =>
            {
                await ReadHttpAsync(nameof(ParallelForeachAsync), name);
                //await WriteFileAsync(name, name);
            });
        }
    }
}
