namespace Maroontress.Collection.Test;

using Maroontress.Collection;

[TestClass]
public sealed class HashTableConstantsTest
{
    [TestMethod]
    public void GetCapacity()
    {
        var m = 0x100;
        var c = new HashTableConstants(m, int.MaxValue, int.MaxValue);
        Assert.AreEqual(1, c.GetCapacity(1));
        Assert.AreEqual(2, c.GetCapacity(2));
        Assert.AreEqual(4, c.GetCapacity(3));
        Assert.AreEqual(4, c.GetCapacity(4));
        Assert.AreEqual(16, c.GetCapacity(15));
        Assert.AreEqual(16, c.GetCapacity(16));
        Assert.AreEqual(32, c.GetCapacity(17));
        Assert.AreEqual(m, c.GetCapacity(m - 1));
        Assert.AreEqual(m, c.GetCapacity(m));
        Assert.AreEqual(m, c.GetCapacity(m + 1));
    }

    [TestMethod]
    public void GetCapacity_Zero()
    {
        var c = new HashTableConstants(0x100, int.MaxValue, int.MaxValue);
        Assert.ThrowsException<ArgumentException>(
            () => c.GetCapacity(0));
    }

    [TestMethod]
    public void MaxCapacityIsZero()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _ = new HashTableConstants(0, int.MaxValue, int.MaxValue));
    }

    [TestMethod]
    public void MaxSizeIsZero()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _ = new HashTableConstants(0x100, 0, int.MaxValue));
    }

    [TestMethod]
    public void MaxRoastIsZero()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _ = new HashTableConstants(0x100, int.MaxValue, 0));
    }
}
