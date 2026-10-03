using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.SwapNodesInPairs;

namespace DSAExperimentation.LeetCode.Tests.SwapNodesInPairs;

// Harness only. Both strategies are SwapNodesInPairsSolution's - this file pins
// them to LeetCode's published examples, stated once as raw values so a fresh
// SinglyLinkedListNode<int> chain is built per assertion: PointerRewiring rewires
// the very nodes it is handed, so reusing one already-swapped instance across the
// two theories sharing this data would silently feed the second call an
// already-consumed structure.
public sealed partial class SwapNodesInPairsSolutionTests
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
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                SwapNodesInPairsSolution.SwapPairsByPointerRewiring(LeetCodeWireFormat.ToLinkedList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SwapPairsByArrayRoundTrip_LeetCodeExamples_SwapsAdjacentPairs(int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                SwapNodesInPairsSolution.SwapPairsByArrayRoundTrip(LeetCodeWireFormat.ToLinkedList(values))));
}
