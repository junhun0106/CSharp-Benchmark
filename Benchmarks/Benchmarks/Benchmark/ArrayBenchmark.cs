using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmark;

public class ArrayFindBenchmark : ArrayBenchmarkBase
    {
        [Benchmark]
        public void ArrayFind()
        {
            var _ = _list.Find(x => x.Value == "a");
        }

        [Benchmark]
        public void FirstOrDefault()
        {
            var _ = _list.FirstOrDefault(x => x.Value == "a");
        }
    }

public class ArrayFindAllBenchmark : ArrayBenchmarkBase
    {
        [Benchmark]
        public void ArrayFindAll()
        {
            var a = _list.FindAll(x => x.Value == "a");
        }

        [Benchmark]
        public void Where()
        {
            var a = _list.Where(x => x.Value == "a");
        }
    }

public class XArrayContainsBenchmark : ArrayBenchmarkBase
    {
        private Input _input;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _input = new Input("c");
            _list = new Input[] {
                new Input("a"),
                new Input("a"),
                new Input("a"),
                new Input("a"),
                new Input("a"),
                new Input("a"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
                _input,
            };
        }

        [Benchmark]
        public void BinarySearch()
        {
            if (_list.Exists_3(_input))
            {
                //throw new Exception();
            }
            else
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void Foreach()
        {
            if (_list.Exists(_input))
            {
                //throw new Exception();
            }
            else
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void For()
        {
            if (_list.Exists_2(_input))
            {
                //throw new Exception();
            }
            else
            {
                throw new Exception();
            }
        }

        //[Benchmark]
        //public void ForStringOrdinalComparer()
        //{
        //    if (_list.Exists_2(_input)) {
        //        throw new Exception();
        //    }
        //}

        [Benchmark]
        public void ContainsComparerNull()
        {
            if (_list.Contains(_input, comparer: null))
            {
                //throw new Exception();
            }
            else
            {
                throw new Exception();
            }
        }

        //[Benchmark]
        //public void ContainsComparerStringOrdinal()
        //{
        //    if (_list.Contains(_input, StringComparer.Ordinal)) {
        //        throw new Exception();
        //    }
        //}
    }

public class ArrayCopyBenchmark : BenchmarkBase
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