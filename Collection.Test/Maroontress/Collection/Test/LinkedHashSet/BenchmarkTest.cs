namespace Maroontress.Collection.Test.LinkedHashSet;

using System.Diagnostics;

[TestClass]
public sealed class BenchmarkTest
{
    private const int TryCount = 10;

    private const int Capacity = 100_000;

    private const int ArraySize = 50_000;

    private static readonly string[] Left = NewArray(0, ArraySize);

    private static readonly string[] SmallLeft = NewArray(1, ArraySize - 1);

    private static readonly string[] BigLeft = NewArray(0, ArraySize + 1);

    private static readonly string[] Right
        = NewArray(ArraySize / 2, ArraySize);

    [TestMethod]
    public void AddAndRehash()
    {
        static TimeSpan Trial<T>(Func<string[], int, T> supply, int n)
            => LapTime(() => _ = supply(Left, n));

        TripleTrial(
            n => Trial(NewHashSet, n),
            n => Trial(NewSimpleLhs, n),
            n => Trial(NewLhs, n),
            16);
    }

    [TestMethod]
    public void Add()
    {
        static TimeSpan Trial<T>(Func<string[], int, T> supply, int n)
            => LapTime(() => _ = supply(Left, n));

        TripleTrial(
            n => Trial(NewHashSet, n),
            n => Trial(NewSimpleLhs, n),
            n => Trial(NewLhs, n));
    }

    [TestMethod]
    public void IsSupersetOf()
    {
        TimeSpan Trial(Func<string[], int, ISet<string>> newSet, int n)
        {
            var s = newSet(Left, n);
            return LapTime(() => Assert.IsTrue(s.IsSupersetOf(SmallLeft)));
        }

        TripleTrial(
            n => Trial(NewHashSet, n),
            n => Trial(NewSimpleLhs, n),
            n => Trial(NewLhs, n));
    }

    [TestMethod]
    public void IsSupersetOf_Set()
    {
        TimeSpan Trial<T>(
            Func<string[], int, T> newSet, int n, Func<T, T, bool> test)
        {
            var s = newSet(Left, n);
            var t = newSet(SmallLeft, n);
            return LapTime(() => Assert.IsTrue(test(s, t)));
        }

        TripleTrial(
            n => Trial(NewHashSet, n, (s, t) => s.IsSupersetOf(t)),
            n => Trial(NewSimpleLhs, n, (s, t) => s.IsSupersetOf(t)),
            n => Trial(NewLhs, n, (s, t) => s.IsSupersetOf(t)));
    }

    [TestMethod]
    public void IsSubsetOf()
    {
        TimeSpan Trial(Func<string[], int, ISet<string>> newSet, int n)
        {
            var s = newSet(Left, n);
            return LapTime(() => Assert.IsTrue(s.IsSubsetOf(BigLeft)));
        }

        TripleTrial(
            n => Trial(NewHashSet, n),
            n => Trial(NewSimpleLhs, n),
            n => Trial(NewLhs, n));
    }

    [TestMethod]
    public void IsSubsetOf_Set()
    {
        TimeSpan Trial<T>(
            Func<string[], int, T> newSet, int n, Func<T, T, bool> test)
        {
            var s = newSet(Left, n);
            var t = newSet(BigLeft, n);
            return LapTime(() => Assert.IsTrue(test(s, t)));
        }

        TripleTrial(
            n => Trial(NewHashSet, n, (s, t) => s.IsSubsetOf(t)),
            n => Trial(NewSimpleLhs, n, (s, t) => s.IsSubsetOf(t)),
            n => Trial(NewLhs, n, (s, t) => s.IsSubsetOf(t)));
    }

    [TestMethod]
    public void IsProperSubsetOf()
    {
        TimeSpan Trial(Func<string[], int, ISet<string>> newSet, int n)
        {
            var s = newSet(Left, n);
            return LapTime(() => Assert.IsTrue(s.IsProperSubsetOf(BigLeft)));
        }

        TripleTrial(
            n => Trial(NewHashSet, n),
            n => Trial(NewSimpleLhs, n),
            n => Trial(NewLhs, n));
    }

    [TestMethod]
    public void IsProperSubsetOf_Set()
    {
        TimeSpan Trial<T>(
            Func<string[], int, T> newSet, int n, Func<T, T, bool> test)
        {
            var s = newSet(Left, n);
            var t = newSet(BigLeft, n);
            return LapTime(() => Assert.IsTrue(test(s, t)));
        }

        TripleTrial(
            n => Trial(NewHashSet, n, (s, t) => s.IsProperSubsetOf(t)),
            n => Trial(NewSimpleLhs, n, (s, t) => s.IsProperSubsetOf(t)),
            n => Trial(NewLhs, n, (s, t) => s.IsProperSubsetOf(t)));
    }

    [TestMethod]
    public void IntersectWith()
    {
        TimeSpan Trial(Func<string[], int, ISet<string>> newSet, int n)
        {
            var s = newSet(Left, n);
            return LapTime(() => s.IntersectWith(Right));
        }

        TripleTrial(
            n => Trial(NewHashSet, n),
            n => Trial(NewSimpleLhs, n),
            n => Trial(NewLhs, n));
    }

    [TestMethod]
    public void IntersectWith_Set()
    {
        TimeSpan Trial<T>(
            Func<string[], int, T> newSet, int n, Action<T, T> apply)
        {
            var s = newSet(Left, n);
            var t = newSet(Right, n);
            return LapTime(() => apply(s, t));
        }

        TripleTrial(
            n => Trial(NewHashSet, n, (s, t) => s.IntersectWith(t)),
            n => Trial(NewSimpleLhs, n, (s, t) => s.IntersectWith(t)),
            n => Trial(NewLhs, n, (s, t) => s.IntersectWith(t)));
    }

    [TestMethod]
    public void SymmetricExceptWith()
    {
        TimeSpan Trial(Func<string[], int, ISet<string>> newSet, int n)
        {
            var s = newSet(Left, n);
            return LapTime(() => s.SymmetricExceptWith(Right));
        }

        TripleTrial(
            n => Trial(NewHashSet, n),
            n => Trial(NewSimpleLhs, n),
            n => Trial(NewLhs, n));
    }

    [TestMethod]
    public void SymmetricExceptWith_Set()
    {
        TimeSpan Trial<T>(
            Func<string[], int, T> newSet, int n, Action<T, T> apply)
        {
            var s = newSet(Left, n);
            var t = newSet(Right, n);
            return LapTime(() => apply(s, t));
        }

        TripleTrial(
            n => Trial(NewHashSet, n, (s, t) => s.SymmetricExceptWith(t)),
            n => Trial(NewSimpleLhs, n, (s, t) => s.SymmetricExceptWith(t)),
            n => Trial(NewLhs, n, (s, t) => s.SymmetricExceptWith(t)));
    }

    private static void TripleTrial(
        Func<int, TimeSpan> a,
        Func<int, TimeSpan> b,
        Func<int, TimeSpan> c,
        int capacity = Capacity)
    {
        double GetResult(Func<int, TimeSpan> apply)
        {
            return Enumerable.Range(0, TryCount)
                .Select(i => apply(capacity).Milliseconds)
                .Average();
        }

        Console.WriteLine($"HashSet: {GetResult(a)}");
        Console.WriteLine($"Conventional: {GetResult(b)}");
        Console.WriteLine($"LinkedHashSet: {GetResult(c)}");
    }

    private static TimeSpan LapTime(Action action)
    {
        GC.Collect(2, GCCollectionMode.Forced, true, true);
        var w = new Stopwatch();
        w.Start();
        action();
        w.Stop();
        return w.Elapsed;
    }

    private static HashSet<string> NewHashSet(
        string[] array, int initialCapacity)
    {
        var s = new HashSet<string>(initialCapacity);
        foreach (var i in array)
        {
            s.Add(i);
        }
        return s;
    }

    private static SimpleLinkedHashSet<string> NewSimpleLhs(
        string[] array, int initialCapacity)
    {
        var s = new SimpleLinkedHashSet<string>(initialCapacity);
        foreach (var i in array)
        {
            s.Add(i);
        }
        return s;
    }

    private static LinkedHashSet<string> NewLhs(
        string[] array, int initialCapacity)
    {
        var s = new LinkedHashSet<string>(initialCapacity);
        foreach (var i in array)
        {
            s.Add(i);
        }
        return s;
    }

    private static string[] NewArray(int n, int size)
        => Enumerable.Range(n, size)
            .Select(i => i.ToString())
            .Where(s => s.GetHashCode() is not 0)
            .ToArray();
}
