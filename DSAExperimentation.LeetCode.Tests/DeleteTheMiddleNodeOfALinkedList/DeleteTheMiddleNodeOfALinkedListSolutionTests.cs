using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.DeleteTheMiddleNodeOfALinkedList;

namespace DSAExperimentation.LeetCode.Tests.DeleteTheMiddleNodeOfALinkedList;

// Harness only. Both strategies are DeleteTheMiddleNodeOfALinkedListSolution's -
// this file states LeetCode's published examples once and pins each strategy to
// them. SinglyLinkedListNode<int> is internal, so it cannot appear in a public
// TheoryData member; the examples travel as value arrays and
// LeetCodeWireFormat.ToLinkedList reconstructs a fresh list inside each test
// method, which every arm needs anyway because deleting is destructive.
public sealed partial class DeleteTheMiddleNodeOfALinkedListSolutionTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 3, 4, 7, 1, 2, 6], [1, 3, 4, 1, 2, 6] }, // LeetCode's example 1: odd length, the single middle goes
            { [1, 2, 3, 4], [1, 2, 4] },                   // LeetCode's example 2: even length, the second of the two middles goes
            { [2, 1], [2] },                               // LeetCode's example 3: two nodes, only the head survives
            { [1], [] },                                   // the one-node list empties
            { [1, 2, 3], [1, 3] },                         // shortest list with a middle that is neither head nor tail
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteMiddleByCountThenRebuild_LeetCodeExamples_RemovesTheNodeAtHalfTheLength(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                DeleteTheMiddleNodeOfALinkedListSolution.DeleteMiddleByCountThenRebuild(
                    LeetCodeWireFormat.ToLinkedList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteMiddleBySlowFastPointers_LeetCodeExamples_RemovesTheNodeAtHalfTheLength(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                DeleteTheMiddleNodeOfALinkedListSolution.DeleteMiddleBySlowFastPointers(
                    LeetCodeWireFormat.ToLinkedList(values))));
}
