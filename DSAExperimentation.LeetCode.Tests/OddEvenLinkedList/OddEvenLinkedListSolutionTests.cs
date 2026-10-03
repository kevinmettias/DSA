using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.OddEvenLinkedList;

namespace DSAExperimentation.LeetCode.Tests.OddEvenLinkedList;

// Harness only. Both strategies are OddEvenLinkedListSolution's - this file just
// pins them to LeetCode's published examples, plus the empty- and single-node
// edge cases neither strategy may special-case incorrectly.
public sealed partial class OddEvenLinkedListSolutionTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], [1, 3, 5, 2, 4] },
            { [2, 1, 3, 5, 6, 4, 7], [2, 3, 6, 7, 1, 5, 4] },
            { [], [] },
            { [42], [42] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GroupOddEvenByTwoListRebuild_LeetCodeExamples_InterleavesOddThenEvenIndices(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                OddEvenLinkedListSolution.GroupOddEvenByTwoListRebuild(LeetCodeWireFormat.ToLinkedList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GroupOddEvenByInPlaceRewire_LeetCodeExamples_InterleavesOddThenEvenIndices(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                OddEvenLinkedListSolution.GroupOddEvenByInPlaceRewire(LeetCodeWireFormat.ToLinkedList(values))));
}
