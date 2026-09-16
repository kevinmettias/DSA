using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.SwapNodesInPairs;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SwapNodesInPairs;

// Harness only. Both strategies are SwapNodesInPairsSolution's - this file pins
// them to LeetCode's published examples, stated once as raw values so a fresh
// SinglyLinkedListNode<int> chain is built per assertion: PointerRewiring rewires
// the very nodes it is handed, so reusing one already-swapped instance across the
// two theories sharing this data would silently feed the second call an
// already-consumed structure.
public sealed partial class SwapNodesInPairsTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4], [2, 1, 4, 3] },
            { [], [] },
            { [1], [1] },
            { [1, 2, 3], [2, 1, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SwapPairsByPointerRewiring_LeetCodeExamples_SwapsAdjacentPairs(int[] values, int[] expected) =>
        Assert.Equal(expected, ToArray(SwapNodesInPairsSolution.SwapPairsByPointerRewiring(BuildList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SwapPairsByArrayRoundTrip_LeetCodeExamples_SwapsAdjacentPairs(int[] values, int[] expected) =>
        Assert.Equal(expected, ToArray(SwapNodesInPairsSolution.SwapPairsByArrayRoundTrip(BuildList(values))));

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
