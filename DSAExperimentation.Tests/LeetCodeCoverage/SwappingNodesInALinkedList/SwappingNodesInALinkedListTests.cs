using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.SwappingNodesInALinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SwappingNodesInALinkedList;

// Harness only. Both strategies are SwappingNodesInALinkedListSolution's - this
// file pins them to LeetCode's published examples plus the boundary cases the
// published pair does not reach: kthPosition = 1 and kthPosition = n (the two ends
// swap with each other), the single-node list, and the odd-length list where the kth
// node from each end is the same node, so the swap has to be a no-op rather than a
// self-assignment that corrupts it.
//
// The examples are stated as raw values, not as built nodes, because
// SwapNodesByTwoPointerWalk rewrites the Values of the very list it is handed:
// sharing one already-swapped chain between the two theories over this data would
// feed the second call an input the first had already consumed.
public sealed class SwappingNodesInALinkedListTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], 2, [1, 4, 3, 2, 5] },
            { [7, 9, 6, 6, 7, 8, 3, 0, 9, 5], 5, [7, 9, 6, 6, 8, 7, 3, 0, 9, 5] },
            { [7, 9, 6, 6, 7, 8, 3, 0, 9, 5], 1, [5, 9, 6, 6, 7, 8, 3, 0, 9, 7] },
            { [1, 2, 3, 4, 5], 5, [5, 2, 3, 4, 1] },
            { [1, 2, 3], 2, [1, 2, 3] },
            { [42], 1, [42] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SwapNodesByArrayMaterialize_LeetCodeExamples_SwapsKthFromFrontAndEnd(
        int[] values, int kthPosition, int[] expected)
    {
        var head = BuildList(values);
        var swapped = SwappingNodesInALinkedListSolution.SwapNodesByArrayMaterialize(head, kthPosition);
        var actual = ToArray(swapped);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SwapNodesByTwoPointerWalk_LeetCodeExamples_SwapsKthFromFrontAndEnd(
        int[] values, int kthPosition, int[] expected)
    {
        var head = BuildList(values);
        var swapped = SwappingNodesInALinkedListSolution.SwapNodesByTwoPointerWalk(head, kthPosition);
        var actual = ToArray(swapped);

        Assert.Equal(expected, actual);
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }

    private static int[] ToArray(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values.ToArray();
    }
}
