namespace Maroontress.Collection.Test;

using System.Collections;
using System.Collections.Immutable;

public abstract class AbstractImmutableDictionaryConformanceTest
{
    [TestMethod]
    public void TryGetKey()
    {
        var empty = NewMap();
        var m = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        Assert.IsTrue(m.TryGetKey(123, out var k123));
        Assert.AreEqual(123, k123);
        Assert.IsFalse(m.TryGetKey(12, out _));
    }

    [TestMethod]
    public void TryGetValue()
    {
        var empty = NewMap();
        var m = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        Assert.IsTrue(m.TryGetValue(123, out var v123));
        Assert.AreEqual("123", v123);
        Assert.IsFalse(m.TryGetValue(12, out _));
    }

    [TestMethod]
    public void GetEnumerator()
    {
        var empty = NewMap();
        var m = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        var d = m.ToDictionary(p => p.Key, p => p.Value);
        Assert.AreEqual(3, d.Count);
        Assert.AreEqual("123", d[123]);
        Assert.AreEqual("456", d[456]);
        Assert.AreEqual("789", d[789]);
    }

    [TestMethod]
    public void GetEnumerator_IEnumerator()
    {
        var empty = NewMap();
        var m = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        var all = (IEnumerable)m;
        var e = all.GetEnumerator();
        var d = new Dictionary<int, string>();
        while (e.MoveNext())
        {
            if (!(e.Current is KeyValuePair<int, string> p))
            {
                throw new AssertFailedException();
            }
            d[p.Key] = p.Value;
        }
        Assert.AreEqual(3, d.Count);
        Assert.AreEqual("123", d[123]);
        Assert.AreEqual("456", d[456]);
        Assert.AreEqual("789", d[789]);
    }

    [TestMethod]
    public void Contains()
    {
        var empty = NewMap();
        var m = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        Assert.IsTrue(m.Contains(NewPair(123)));
        Assert.IsTrue(m.Contains(NewPair(456)));
        Assert.IsTrue(m.Contains(NewPair(789)));
        Assert.IsFalse(m.Contains(NewPair(12)));
        Assert.IsFalse(m.Contains(NewPair(123, "12")));
    }

    [TestMethod]
    public void ContainsKey()
    {
        var empty = NewMap();
        var m = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        Assert.IsTrue(m.ContainsKey(123));
        Assert.IsTrue(m.ContainsKey(456));
        Assert.IsTrue(m.ContainsKey(789));
        Assert.IsFalse(m.ContainsKey(1));
    }

    [TestMethod]
    public void SetItems_Null()
    {
        var empty = NewMap();
        Assert.ThrowsException<ArgumentNullException>(
            () => _ = empty.SetItems(null!));
    }

    [TestMethod]
    public void SetItems_KeyDoesNotExist()
    {
        var empty = NewMap();
        var m1 = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        var m2 = m1.SetItems(
            ImmutableArray.Create(
                NewPair(12),
                NewPair(45),
                NewPair(12, "78")));
        Assert.AreEqual(5, m2.Count);
        Assert.AreEqual("123", m2[123]);
        Assert.AreEqual("456", m2[456]);
        Assert.AreEqual("789", m2[789]);
        Assert.AreEqual("78", m2[12]);
        Assert.AreEqual("45", m2[45]);
    }

    [TestMethod]
    public void SetItems_Empty()
    {
        var empty = NewMap();
        var m1 = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        var m2 = m1.SetItems(empty);
        Assert.AreSame(m1, m2);
    }

    [TestMethod]
    public void SetItems_Overwrite()
    {
        var empty = NewMap();
        var m1 = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        var m2 = m1.SetItems(
            ImmutableArray.Create(
                NewPair(789, "78"),
                NewPair(456, "45"),
                NewPair(123, "12")));
        Assert.AreEqual(3, m2.Count);
        Assert.AreEqual("12", m2[123]);
        Assert.AreEqual("45", m2[456]);
        Assert.AreEqual("78", m2[789]);
    }

    [TestMethod]
    public void SetItems_ReturnThis()
    {
        var empty = NewMap();
        var m1 = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        var m2 = m1.SetItems(
            ImmutableArray.Create(789, 456, 123)
                .Select(NewPair));
        Assert.AreSame(m1, m2);
    }

    [TestMethod]
    public void SetItems()
    {
        var empty = NewMap();
        var m1 = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        var m2 = m1.SetItems(
            ImmutableArray.Create(
                NewPair(12),
                NewPair(456),
                NewPair(123, "23")));
        Assert.AreEqual(4, m2.Count);
        Assert.AreEqual("23", m2[123]);
        Assert.AreEqual("456", m2[456]);
        Assert.AreEqual("789", m2[789]);
        Assert.AreEqual("12", m2[12]);
    }

    [TestMethod]
    public void SetItems_AppendOnly()
    {
        var empty = NewMap();
        var m1 = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        var m2 = m1.SetItems(
            ImmutableArray.Create(
                NewPair(12),
                NewPair(45)));
        Assert.AreEqual(5, m2.Count);
        Assert.AreEqual("123", m2[123]);
        Assert.AreEqual("456", m2[456]);
        Assert.AreEqual("789", m2[789]);
        Assert.AreEqual("12", m2[12]);
        Assert.AreEqual("45", m2[45]);
    }

    [TestMethod]
    public void SetItem_Overwrite()
    {
        var empty = NewMap();
        var m1 = empty.Add(123, "123");
        var m2 = m1.SetItem(123, "456");
        Assert.AreEqual(1, m2.Count);
        Assert.AreEqual("456", m2[123]);
    }

    [TestMethod]
    public void SetItem_KeyDoesNotExist()
    {
        var empty = NewMap();
        var m1 = empty.Add(123, "123");
        var m2 = m1.SetItem(12, "12");
        Assert.AreEqual(2, m2.Count);
        Assert.AreEqual("123", m2[123]);
        Assert.AreEqual("12", m2[12]);
    }

    [TestMethod]
    public void SetItem_ReturnThis()
    {
        var empty = NewMap();
        var m1 = empty.Add(123, "123");
        var m2 = m1.SetItem(123, "123");
        Assert.AreSame(m1, m2);
    }

    [TestMethod]
    public void RemoveRange()
    {
        var empty = NewMap();
        var m1 = empty.AddRange(
            ImmutableArray.Create(123, 456, 789, 12)
                .Select(NewPair));
        var m2 = m1.RemoveRange(
            ImmutableArray.Create(456, 789));
        Assert.AreEqual(2, m2.Count);
        Assert.AreEqual("123", m2[123]);
        Assert.AreEqual("12", m2[12]);
        Assert.IsFalse(m2.ContainsKey(456));
        Assert.IsFalse(m2.ContainsKey(789));
    }

    [TestMethod]
    public void RemoveRange_Null()
    {
        var empty = NewMap();
        Assert.ThrowsException<ArgumentNullException>(
            () => _ = empty.RemoveRange(null!));
    }

    [TestMethod]
    public void RemoveRange_Empty()
    {
        var empty = NewMap();
        var m1 = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        var m2 = m1.RemoveRange(Array.Empty<int>());
        Assert.AreSame(m1, m2);
    }

    [TestMethod]
    public void RemoveRange_ReturnThis()
    {
        var empty = NewMap();
        var m1 = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        var m2 = m1.RemoveRange(
            ImmutableArray.Create(12, 34, 56));
        Assert.AreSame(m1, m2);
    }

    [TestMethod]
    public void Remove_ReturnThis()
    {
        var empty = NewMap();
        var m1 = empty.Add(123, "123");
        var m2 = m1.Remove(456);
        Assert.AreSame(m1, m2);
    }

    [TestMethod]
    public void Remove()
    {
        var empty = NewMap();
        var m1 = empty.AddRange(
            ImmutableArray.Create(123, 456, 789)
                .Select(NewPair));
        var m2 = m1.Remove(456);
        Assert.AreEqual(2, m2.Count);
        Assert.AreEqual("123", m2[123]);
        Assert.AreEqual("789", m2[789]);
        Assert.IsFalse(m2.ContainsKey(456));
    }

    [TestMethod]
    public void AddRange_KeyExistsButHasDifferentValue()
    {
        var empty = NewMap();
        var m1 = empty.Add(123, "123");
        var delta = ImmutableArray.Create(
            NewPair(456),
            NewPair(789),
            NewPair(123, "12"));
        Assert.ThrowsException<ArgumentException>(
            () => _ = m1.AddRange(delta));
    }

    [TestMethod]
    public void AddRange_Null()
    {
        var empty = NewMap();
        Assert.ThrowsException<ArgumentNullException>(
            () => _ = empty.AddRange(null!));
    }

    [TestMethod]
    public void AddRange()
    {
        var empty = NewMap();
        var m = empty.AddRange(
            ImmutableArray.Create(123, 456, 789, 789, 456, 123)
                .Select(NewPair));
        Assert.AreEqual(3, m.Count);
        Assert.AreEqual("123", m[123]);
        Assert.AreEqual("456", m[456]);
        Assert.AreEqual("789", m[789]);
    }

    [TestMethod]
    public void AddRange_ReturnThis()
    {
        var empty = NewMap();
        var m1 = empty.Add(123, "123");
        var m2 = m1.AddRange(
            ImmutableArray.Create(
                NewPair(123),
                NewPair(123, "123")));
        Assert.AreSame(m1, m2);
    }

    [TestMethod]
    public void AddRange_Empty()
    {
        var empty = NewMap();
        var m1 = empty.Add(123, "123");
        var m2 = m1.AddRange(empty);
        Assert.AreSame(m1, m2);
    }

    [TestMethod]
    public void Add_ReturnThis()
    {
        var empty = NewMap();
        var m1 = empty.Add(123, "123");
        var m2 = m1.Add(123, "123");
        var m3 = m1.Add(123, 123.ToString());
        Assert.AreSame(m1, m2);
        Assert.AreSame(m1, m3);
    }

    [TestMethod]
    public void Add_KeyExistsButHasDifferentValue()
    {
        var empty = NewMap();
        var m1 = empty.Add(123, "123");
        Assert.ThrowsException<ArgumentException>(
            () => _ = m1.Add(123, "456"));
    }

    protected abstract IImmutableDictionary<int, string> NewMap();

    private static KeyValuePair<int, string> NewPair(int i)
        => new(i, i.ToString());

    private static KeyValuePair<int, string> NewPair(int i, string s)
        => new(i, s);
}
