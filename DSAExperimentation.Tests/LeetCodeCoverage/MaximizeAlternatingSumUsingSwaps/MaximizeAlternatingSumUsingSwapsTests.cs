using DSAExperimentation.LeetCode.MaximizeAlternatingSumUsingSwaps;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeAlternatingSumUsingSwaps;

// Harness only. MaximizeAlternatingSumUsingSwapsSolution owns both the BFS baseline
// and the DisjointSet strategy; this file pins them to LeetCode's three published
// examples, including the no-swaps-at-all case where every index is its own
// singleton component.
public sealed class MaximizeAlternatingSumUsingSwapsTests
{
    public static TheoryData<int[], int[][], long> Examples =>
        new()
        {
            { [1, 2, 3], [[0, 2], [1, 2]], 4L },
            { [1, 2, 3], [[1, 2]], 2L },
            { [1, 1_000_000_000, 1, 1_000_000_000, 1, 1_000_000_000], [], -2_999_999_997L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumAlternatingSumByComponentBfs_LeetCodeExamples_ReturnsMaximumAlternatingSum(
        int[] nums, int[][] swaps, long expected) =>
        Assert.Equal(
            expected, MaximizeAlternatingSumUsingSwapsSolution.MaximumAlternatingSumByComponentBfs(nums, swaps));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumAlternatingSumByDisjointSet_LeetCodeExamples_ReturnsMaximumAlternatingSum(
        int[] nums, int[][] swaps, long expected) =>
        Assert.Equal(
            expected, MaximizeAlternatingSumUsingSwapsSolution.MaximumAlternatingSumByDisjointSet(nums, swaps));
}
