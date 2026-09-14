using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.MergeNodesInBetweenZeros;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeNodesInBetweenZeros;

// Harness only. Both strategies are MergeNodesInBetweenZerosSolution's - including
// the two-pass value buffer, which the benchmark used to own privately as its
// baseline and nothing asserted. SinglyLinkedListNode<int> is internal, so it
// cannot appear in a public TheoryData<...> member (CS0053); the examples state
// the node values and LeetCodeWireFormat translates both ends.
public sealed class MergeNodesInBetweenZerosTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            // LC example 1: groups 3 + 1 and 4 + 5 + 2.
            { [0, 3, 1, 0, 4, 5, 2, 0], [4, 11] },

            // LC example 2: groups 1 and 3 + 4.
            { [0, 1, 0, 3, 4, 0], [1, 7] },

            // The shortest list LC allows: one group of one node.
            { [0, 5, 0], [5] },

            // Three groups of differing lengths, so no strategy can pass by
            // assuming a fixed group size.
            { [0, 1, 2, 3, 0, 4, 0, 5, 6, 0], [6, 4, 11] },

            // LC's largest node value, and a group that follows it, so the merged
            // sum of one group cannot leak into the next.
            { [0, 1000, 0, 1, 1, 1, 0], [1000, 3] },

            // Every group is a single node, so the answer is as long as the input
            // is short.
            { [0, 7, 0, 8, 0, 9, 0], [7, 8, 9] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeNodesByTwoPassValueBuffer_LeetCodeExamples_ReturnsOneNodePerGroupSum(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                MergeNodesInBetweenZerosSolution.MergeNodesByTwoPassValueBuffer(
                    LeetCodeWireFormat.ToLinkedList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeNodesBySinglePassSum_LeetCodeExamples_ReturnsOneNodePerGroupSum(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                MergeNodesInBetweenZerosSolution.MergeNodesBySinglePassSum(
                    LeetCodeWireFormat.ToLinkedList(values))));
}
