using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.CopyListWithRandomPointer;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CopyListWithRandomPointer;

// Harness only. RandomLinkedListNode<TValue> is a DataStructures/SinglyLinkedList
// representation and CopyListWithRandomPointerSolution.CopyByHashMapMemo is the
// one strategy this file pins to LeetCode's published examples - each example is
// (Value, RandomIndex) pairs in list order, RandomIndex null when that node's
// Random pointer is unset, matching LC's own wire format for this problem.
public sealed partial class CopyListWithRandomPointerTests
{
    public static TheoryData<(int Value, int? RandomIndex)[]> Examples =>
        new()
        {
            { [] }, // empty list
            { [(7, null), (13, 0)] }, // the original pre-migration test's two-node case
            { [(7, null), (13, 0), (11, 4), (10, 2), (1, 0)] }, // LC's example 1
            { [(1, 1), (2, 1)] }, // LC's example 2
            { [(3, null), (3, 0), (3, null)] }, // LC's example 3
            { [(5, 0)] }, // a single node whose Random points at itself
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CopyByHashMapMemo_LeetCodeExamples_DeepCopiesNextAndRandomLinks(
        (int Value, int? RandomIndex)[] spec)
    {
        var head = Build(spec);

        var clone = CopyListWithRandomPointerSolution.CopyByHashMapMemo(head);

        Assert.Equal(spec, Flatten(clone));
        AssertNoSharedNodes(head, clone);
    }

    private static RandomLinkedListNode<int>? Build((int Value, int? RandomIndex)[] spec)
    {
        if (spec.Length == 0)
        {
            return null;
        }

        var nodes = spec.Select(s => new RandomLinkedListNode<int>(s.Value)).ToArray();

        for (var i = 0; i < nodes.Length - 1; i++)
        {
            nodes[i].Next = nodes[i + 1];
        }

        LinkRandomPointers(nodes, spec);

        return nodes[0];
    }

    // Each node's Random is its own spec entry's target, or stays at the null it was
    // constructed with when the spec records no Random for it - which is why the
    // absent case needs no assignment of its own.
    private static void LinkRandomPointers(
        RandomLinkedListNode<int>[] nodes, (int Value, int? RandomIndex)[] spec)
    {
        for (var i = 0; i < nodes.Length; i++)
        {
            if (spec[i].RandomIndex is int index)
            {
                nodes[i].Random = nodes[index];
            }
        }
    }

    private static (int Value, int? RandomIndex)[] Flatten(RandomLinkedListNode<int>? head)
    {
        var nodes = new List<RandomLinkedListNode<int>>();

        for (var node = head; node is not null; node = node.Next)
        {
            nodes.Add(node);
        }

        return [.. nodes.Select(n => (n.Value, n.Random is null ? (int?)null : nodes.IndexOf(n.Random)))];
    }

    // Every cloned node must be a genuinely different object from the original it
    // copies - the defect a shallow "just copy the reference" clone would have.
    private static void AssertNoSharedNodes(
        RandomLinkedListNode<int>? original, RandomLinkedListNode<int>? clone)
    {
        while (original is not null && clone is not null)
        {
            Assert.NotSame(original, clone);
            original = original.Next;
            clone = clone.Next;
        }

        Assert.Null(original);
        Assert.Null(clone);
    }
}
