using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.LinkedListCycleII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LinkedListCycleII;

// Harness only. Both strategies are LinkedListCycleIISolution's - this file
// builds LeetCode's published examples as linked lists (pos is the 0-based index
// the tail's Next rejoins, or -1 for no cycle) and checks each strategy returns
// the same node reference as the cycle's entry point.
public sealed class LinkedListCycleIITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [3, 2, 0, -4], 1 },
            { [1, 2], 0 },
            { [1], -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DetectCycleByVisitedSet_LeetCodeExamples_ReturnsTheCycleEntryNode(int[] values, int pos)
    {
        var (head, expectedEntry) = BuildList(values, pos);

        Assert.Same(expectedEntry, LinkedListCycleIISolution.DetectCycleByVisitedSet(head));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void DetectCycleByFloydCycleDetection_LeetCodeExamples_ReturnsTheCycleEntryNode(int[] values, int pos)
    {
        var (head, expectedEntry) = BuildList(values, pos);

        Assert.Same(expectedEntry, LinkedListCycleIISolution.DetectCycleByFloydCycleDetection(head));
    }

    private static (SinglyLinkedListNode<int>? Head, SinglyLinkedListNode<int>? EntryNode) BuildList(
        int[] values, int pos)
    {
        if (values.Length == 0)
        {
            return (null, null);
        }

        var nodes = CreateChain(values);

        if (pos < 0)
        {
            return (nodes[0], null);
        }

        nodes[^1].Next = nodes[pos];
        return (nodes[0], nodes[pos]);
    }

    // The chain in insertion order: each node links to the one after it, so the
    // pair (head, tail) is already a walkable list for pos < 0.
    private static SinglyLinkedListNode<int>[] CreateChain(int[] values)
    {
        var nodes = new SinglyLinkedListNode<int>[values.Length];

        for (var i = 0; i < values.Length; i++)
        {
            nodes[i] = new SinglyLinkedListNode<int>(values[i]);

            if (i > 0)
            {
                nodes[i - 1].Next = nodes[i];
            }
        }

        return nodes;
    }
}
