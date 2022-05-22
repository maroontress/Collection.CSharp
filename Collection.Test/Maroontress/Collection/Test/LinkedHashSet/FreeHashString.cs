namespace Maroontress.Collection.Test.LinkedHashSet;

public sealed class FreeHashString : IComparable<FreeHashString?>
{
    public FreeHashString(string s, int hash)
    {
        Value = s;
        Hash = hash;
    }

    private string Value { get; }

    private int Hash { get; }

    public int CompareTo(FreeHashString? other)
    {
        if (other is null)
        {
            throw new NullReferenceException();
        }
        return Value.CompareTo(other.Value);
    }

    public override bool Equals(object? o)
    {
        return o is FreeHashString s
            && Value.Equals(s.Value);
    }

    public override int GetHashCode()
    {
        return Hash;
    }
}
