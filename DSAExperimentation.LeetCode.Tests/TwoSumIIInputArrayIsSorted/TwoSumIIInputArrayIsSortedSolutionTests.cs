using DSAExperimentation.LeetCode.TwoSumIIInputArrayIsSorted;

namespace DSAExperimentation.LeetCode.Tests.TwoSumIIInputArrayIsSorted;

// Harness only. OffsetSequence is DataStructures.Sequence's and both strategies
// here are TwoSumIIInputArrayIsSortedSolution's - this file just pins them to
// LeetCode's published examples, including a duplicate-valued array to prove both
// arms still land on the correct pair, not merely some equal value.
public sealed partial class TwoSumIIInputArrayIsSortedSolutionTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [2, 7, 11, 15], 9, [1, 2] }, // LC's example 1
            { [2, 3, 4], 6, [1, 3] }, // LC's example 2
            { [-1, 0], -1, [1, 2] }, // LC's example 3
            { [1, 2, 3, 4, 4, 9, 56, 90], 8, [4, 5] }, // duplicate values in the array
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TryFindIndicesByBinarySearch_LeetCodeExamples_ReturnsOneBasedIndices(
        int[] nums, int target, int[] expected)
    {
        var indices = TwoSumIIInputArrayIsSortedSolution.TryFindIndicesByBinarySearch(nums, target);

        Assert.Equal(expected, indices);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void TryFindIndicesByTwoPointerSqueeze_LeetCodeExamples_ReturnsOneBasedIndices(
        int[] nums, int target, int[] expected)
    {
        var indices = TwoSumIIInputArrayIsSortedSolution.TryFindIndicesByTwoPointerSqueeze(nums, target);

        Assert.Equal(expected, indices);
    }

    // The two arms are competing strategies for one question, so the property worth
    // pinning is that they report the same pair on every example - not merely that
    // each agrees with the expectation beside it.
    [Theory]
    [MemberData(nameof(Examples))]
    public void TryFindIndices_AgreeOnEveryExample(int[] nums, int target, int[] expected) =>
        Assert.Equal(
            TwoSumIIInputArrayIsSortedSolution.TryFindIndicesByBinarySearch(nums, target),
            TwoSumIIInputArrayIsSortedSolution.TryFindIndicesByTwoPointerSqueeze(nums, target));
}
