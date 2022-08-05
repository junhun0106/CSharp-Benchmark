using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using Cysharp.Text;
using Microsoft.Extensions.ObjectPool;

namespace Benchmarks.Benchmark;

public class StringInternBenchmark : BenchmarkBase
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

public class StringSubStringBenchmark : BenchmarkBase
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

public class InterpolatedStringHandlerTest : BenchmarkBase
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

public static class StringBuilderPool
{
    private static readonly ObjectPool<StringBuilder> _shared = new DefaultObjectPoolProvider().CreateStringBuilderPool();

    public static StringBuilder Get() => _shared.Get();

    public static void Return(StringBuilder sb)
    {
        if (sb != null)
        {
            _shared.Return(sb);
        }
    }
}

// Copied from https://github.com/dotnet/runtime/blob/a9c5eadd951dcba73167f72cc624eb790573663a/src/libraries/Common/src/System/Text/ValueStringBuilder.cs
public ref struct ValueStringBuilder
{
    private char[]? _arrayToReturnToPool;
    private Span<char> _chars;
    private int _pos;

    public ValueStringBuilder(Span<char> initialBuffer)
    {
        _arrayToReturnToPool = null;
        _chars = initialBuffer;
        _pos = 0;
    }

    public ValueStringBuilder(int initialCapacity)
    {
        _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
        _chars = _arrayToReturnToPool;
        _pos = 0;
    }

    public int Length
    {
        get => _pos;
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= _chars.Length);
            _pos = value;
        }
    }

    public int Capacity => _chars.Length;

    public void EnsureCapacity(int capacity)
    {
        // This is not expected to be called this with negative capacity
        Debug.Assert(capacity >= 0);

        // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
        if ((uint)capacity > (uint)_chars.Length)
            Grow(capacity - _pos);
    }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// Does not ensure there is a null char after <see cref="Length"/>
    /// This overload is pattern matched in the C# 7.3+ compiler so you can omit
    /// the explicit method call, and write eg "fixed (char* c = builder)"
    /// </summary>
    public ref char GetPinnableReference()
    {
        return ref MemoryMarshal.GetReference(_chars);
    }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
    public ref char GetPinnableReference(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _chars[Length] = '\0';
        }
        return ref MemoryMarshal.GetReference(_chars);
    }

    public ref char this[int index]
    {
        get
        {
            Debug.Assert(index < _pos);
            return ref _chars[index];
        }
    }

    public override string ToString()
    {
        string s = _chars.Slice(0, _pos).ToString();
        Dispose();
        return s;
    }

    /// <summary>Returns the underlying storage of the builder.</summary>
    public Span<char> RawChars => _chars;

    /// <summary>
    /// Returns a span around the contents of the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
    public ReadOnlySpan<char> AsSpan(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _chars[Length] = '\0';
        }
        return _chars.Slice(0, _pos);
    }

    public ReadOnlySpan<char> AsSpan() => _chars.Slice(0, _pos);
    public ReadOnlySpan<char> AsSpan(int start) => _chars.Slice(start, _pos - start);
    public ReadOnlySpan<char> AsSpan(int start, int length) => _chars.Slice(start, length);

    public bool TryCopyTo(Span<char> destination, out int charsWritten)
    {
        if (_chars.Slice(0, _pos).TryCopyTo(destination))
        {
            charsWritten = _pos;
            Dispose();
            return true;
        }
        else
        {
            charsWritten = 0;
            Dispose();
            return false;
        }
    }

    public void Insert(int index, char value, int count)
    {
        if (_pos > _chars.Length - count)
        {
            Grow(count);
        }

        int remaining = _pos - index;
        _chars.Slice(index, remaining).CopyTo(_chars.Slice(index + count));
        _chars.Slice(index, count).Fill(value);
        _pos += count;
    }

    public void Insert(int index, string? s)
    {
        if (s == null)
        {
            return;
        }

        int count = s.Length;

        if (_pos > (_chars.Length - count))
        {
            Grow(count);
        }

        int remaining = _pos - index;
        _chars.Slice(index, remaining).CopyTo(_chars.Slice(index + count));
        s.AsSpan().CopyTo(_chars.Slice(index));
        _pos += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c)
    {
        int pos = _pos;
        if ((uint)pos < (uint)_chars.Length)
        {
            _chars[pos] = c;
            _pos = pos + 1;
        }
        else
        {
            GrowAndAppend(c);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string? s)
    {
        if (s == null)
        {
            return;
        }

        int pos = _pos;
        if (s.Length == 1 && (uint)pos < (uint)_chars.Length) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
        {
            _chars[pos] = s[0];
            _pos = pos + 1;
        }
        else
        {
            AppendSlow(s);
        }
    }

    private void AppendSlow(string s)
    {
        int pos = _pos;
        if (pos > _chars.Length - s.Length)
        {
            Grow(s.Length);
        }

        s.AsSpan().CopyTo(_chars.Slice(pos));
        _pos += s.Length;
    }

    public void Append(char c, int count)
    {
        if (_pos > _chars.Length - count)
        {
            Grow(count);
        }

        Span<char> dst = _chars.Slice(_pos, count);
        for (int i = 0; i < dst.Length; i++)
        {
            dst[i] = c;
        }
        _pos += count;
    }

    public unsafe void Append(char* value, int length)
    {
        int pos = _pos;
        if (pos > _chars.Length - length)
        {
            Grow(length);
        }

        Span<char> dst = _chars.Slice(_pos, length);
        for (int i = 0; i < dst.Length; i++)
        {
            dst[i] = *value++;
        }
        _pos += length;
    }

    public void Append(ReadOnlySpan<char> value)
    {
        int pos = _pos;
        if (pos > _chars.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(_chars.Slice(_pos));
        _pos += value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AppendSpan(int length)
    {
        int origPos = _pos;
        if (origPos > _chars.Length - length)
        {
            Grow(length);
        }

        _pos = origPos + length;
        return _chars.Slice(origPos, length);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(char c)
    {
        Grow(1);
        Append(c);
    }

    /// <summary>
    /// Resize the internal buffer either by doubling current buffer size or
    /// by adding <paramref name="additionalCapacityBeyondPos"/> to
    /// <see cref="_pos"/> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    /// Number of chars requested beyond current position.
    /// </param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(_pos > _chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative
        char[] poolArray = ArrayPool<char>.Shared.Rent((int)Math.Max((uint)(_pos + additionalCapacityBeyondPos), (uint)_chars.Length * 2));

        _chars.Slice(0, _pos).CopyTo(poolArray);

        char[]? toReturn = _arrayToReturnToPool;
        _chars = _arrayToReturnToPool = poolArray;
        if (toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        char[]? toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if (toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }
}

public class StringBuilderAppendBenchMark : BenchmarkBase
{
    private const string testString = "일이삼사오육칠팔구십일이삼사오육칠팔구십일이삼사오육칠팔구십일이삼사오육칠팔구십";

    [Benchmark]
    public void StringBuilderAppend()
    {
        var sb = new StringBuilder();
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
    }

    [Benchmark]
    public void StringBuilderPoolAppend()
    {
        var sb = StringBuilderPool.Get();
        try
        {
            sb.Append(testString);
            sb.Append(testString);
            sb.Append(testString);
            sb.Append(testString);
            sb.Append(testString);
        }
        finally
        {
            StringBuilderPool.Return(sb);
        }
    }

    [Benchmark]
    public void ZStringAppend()
    {
        using var sb = ZString.CreateStringBuilder();
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
    }

    [Benchmark]
    public void ValueStringBuilderAppend()
    {
        var sb = new ValueStringBuilder();
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
        sb.Dispose();
    }
}

public class StringBuilderAppendWithCapacityBenchMark : BenchmarkBase
{
    private string testString;

    [GlobalSetup]
    public void Init()
    {
        const int count = 1000;
        for (int i = 0; i < count; ++i)
        {
            testString += i.ToString();
        }
    }

    [Benchmark]
    public void StringBuilderAppend()
    {
        var sb = new StringBuilder(testString.Length * 5);
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
    }

    [Benchmark]
    public void StringBuilderPoolAppend()
    {
        // 이미 내부에서 Capacity 100으로 지정
        var sb = StringBuilderPool.Get();
        try
        {
            sb.Append(testString);
            sb.Append(testString);
            sb.Append(testString);
            sb.Append(testString);
            sb.Append(testString);
        }
        finally
        {
            StringBuilderPool.Return(sb);
        }
    }

    [Benchmark]
    public void ZStringAppend()
    {
        using var sb = ZString.CreateStringBuilder();
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
    }

    [Benchmark]
    public void ValueStringBuilderAppend()
    {
        using var sb = new ValueStringBuilder(testString.Length * 5);
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
        sb.Append(testString);
    }
}