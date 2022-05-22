namespace Maroontress.Collection.Test.LinkedHashSet;

public sealed class SafeLinkedHashSet<T> : LinkedHashSet<T>
    where T : notnull
{
    public SafeLinkedHashSet(
            int initialCapacity = DefaultInitialCapacity,
            float loadFactor = DefaultLoadFactor)
        : base(initialCapacity, loadFactor)
    {
    }

    public int GetCapacity() => CapacityAndLimit.Capacity;

    public int GetLimit() => CapacityAndLimit.Limit;

    public void Validate()
    {
        if (Count is 0)
        {
            Validate0();
            return;
        }
        var (head, tail) = FirstAndLastNode;
        var nodes = GetNodes().ToArray();
        Assert.IsNotNull(head);
        Assert.IsNotNull(tail);
        Assert.IsNull(head.PreviousNode);
        Assert.IsNull(tail.NextNode);
        {
            var e = head;
            while (e is not null)
            {
                var next = e.NextNode;
                if (next is not null)
                {
                    Assert.AreSame(e, next.PreviousNode);
                }
                else
                {
                    Assert.AreSame(e, tail);
                }
                e = next;
            }
        }
        {
            var indexSet = new HashSet<int>();
            var length = nodes.Length;
            var n = 0;
            var roast = head.Roast;
            var e = head;
            while (e is not null)
            {
                Assert.IsTrue(e.Roast <= roast);
                var i = e.Hash & (length - 1);
                indexSet.Add(i);
                TraceChain(nodes[i]!, e);
                e = e.NextNode;
                ++n;
            }
            Assert.AreEqual(Count, n);
            for (var k = 0; k < length; ++k)
            {
                if (indexSet.Contains(k))
                {
                    continue;
                }
                Assert.IsNull(nodes[k]);
            }
        }
    }

    private static void TraceChain(ProtectedNode root, ProtectedNode e)
    {
        var node = root;
        for (;;)
        {
            if (ReferenceEquals(node, e))
            {
                return;
            }
            node = node!.ParentNode;
        }
    }

    private void Validate0()
    {
        var (head, tail) = FirstAndLastNode;
        Assert.IsNull(head);
        Assert.IsNull(tail);
        var nodes = GetNodes();
        Assert.IsTrue(nodes.All(i => i is null));
    }
}
