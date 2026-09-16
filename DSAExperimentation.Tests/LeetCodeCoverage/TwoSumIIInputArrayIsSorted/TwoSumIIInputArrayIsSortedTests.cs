using DSAExperimentation.LeetCode.TwoSumIIInputArrayIsSorted;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TwoSumIIInputArrayIsSorted;

// Harness only. OffsetSequence is DataStructures.Sequence's and the one strategy
// here is TwoSumIIInputArrayIsSortedSolution's - this file just pins it to
// LeetCode's published examples, including a duplicate-valued array to prove the
// binary search still lands on the correct pair, not merely some equal value.
public sealed partial class TwoSumIIInputArrayIsSortedTests
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
}
