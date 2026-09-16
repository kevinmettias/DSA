using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.RemoveNodesFromLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveNodesFromLinkedList;

// Harness only. Both strategies are RemoveNodesFromLinkedListSolution's - including
// the per-node rescan, which the benchmark used to own privately as its baseline and
// nothing asserted. SinglyLinkedListNode<int> is internal, so it cannot appear in a
// public TheoryData<...> member (CS0053); the examples state the node values and
// LeetCodeWireFormat translates both ends.
public sealed partial class RemoveNodesFromLinkedListTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            // LC example 1.
            { [5, 2, 13, 3, 8], [13, 8] },

            // LC example 2: equal values never remove each other, so nothing goes.
            { [1, 1, 1, 1], [1, 1, 1, 1] },

            // Strictly increasing: every node has a greater one to its right except
            // the last, which is the worst case for the answer's length.
            { [1, 2, 3], [3] },

            // Strictly decreasing: already the answer, so nothing may be dropped.
            { [8, 3, 1], [8, 3, 1] },

            // A single node has nothing to its right and always survives.
            { [1], [1] },

            // Ties around a removal: the middle 2 goes, but neither 5 removes the
            // other, which is what separates a strict < from a <= in the sweep.
            { [5, 5, 2, 5], [5, 5, 5] },

            // The surviving node is the last one, reached only after two removals.
            { [2, 1, 3], [3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveNodesByBruteForceScan_LeetCodeExamples_KeepsOnlyNodesWithNoGreaterValueToTheirRight(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                RemoveNodesFromLinkedListSolution.RemoveNodesByBruteForceScan(
                    LeetCodeWireFormat.ToLinkedList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveNodesByMonotonicStack_LeetCodeExamples_KeepsOnlyNodesWithNoGreaterValueToTheirRight(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                RemoveNodesFromLinkedListSolution.RemoveNodesByMonotonicStack(
                    LeetCodeWireFormat.ToLinkedList(values))));
}
