namespace Benchmarks;

public class Input
{
    public string Value;

    public Input(string value)
    {
        Value = value;
    }
}

public class Output
{
    public string Value;

    public Output(string value)
    {
        Value = value;
    }
}

public enum EType : byte
{
    A = 0,
    B = 1,
    C = 2,
    D = 3,
    E = 4,
}

public interface ITestStruct { }

public readonly struct TestStruct : ITestStruct, IEquatable<TestStruct>
{
    public readonly long Id;
    public readonly EType Type;
    public readonly DateTime Time;
    public readonly string Name;

    public TestStruct(long id, EType type, DateTime time, string name)
    {
        Id = id;
        Type = type;
        Time = time;
        Name = name;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return Id == 0 && Type == default && Time == default && Name is null;
        }

        return obj is TestStruct inst && Equals(inst);
    }

    public bool Equals(TestStruct other)
    {
        return Id == other.Id && Type == other.Type && Time == other.Time && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Type, Name);
    }
}

public sealed class TestStructEqulityComparer : IEqualityComparer<TestStruct>
{
    public static readonly TestStructEqulityComparer Default = new();

    public bool Equals(TestStruct x, TestStruct y)
    {
        return x.Id == y.Id && x.Type == y.Type && x.Name == y.Name;
    }

    public int GetHashCode(TestStruct obj)
    {
        return obj.GetHashCode();
    }
}

/// <summary>
/// 아무것도 구현하지 않은 순수 구조체
/// </summary>
public struct TestStruct_NotImpl : ITestStruct
{
    public long Id;
    public EType Type;
    public DateTime Time;
    public string Name;

    public TestStruct_NotImpl(long id, EType type, DateTime time, string name)
    {
        Id = id;
        Type = type;
        Time = time;
        Name = name;
    }
}