using DSAExperimentation.LeetCode.MaximizeCountOfDistinctPrimesAfterSplit;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeCountOfDistinctPrimesAfterSplit;

// Harness only. Both strategies are
// MaximizeCountOfDistinctPrimesAfterSplitSolution's - this file just pins them
// to LeetCode's published examples, including updates that persist across
// queries and the case where no split ever contains a prime.
public sealed class MaximizeCountOfDistinctPrimesAfterSplitTests
{
    public static TheoryData<int[], int[][], int[]> Examples =>
        new()
        {
            { [2, 1, 3, 1, 2], [[1, 2], [3, 3]], [3, 4] },
            { [2, 1, 4], [[0, 1]], [0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDistinctPrimeCountsByBruteForce_LeetCodeExamples_ReturnsPerQueryMaxSplitPrimeCounts(
        int[] nums, int[][] queries, int[] expected) =>
        Assert.Equal(
            expected,
            MaximizeCountOfDistinctPrimesAfterSplitSolution.MaxDistinctPrimeCountsByBruteForce(nums, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDistinctPrimeCountsByPrefixSuffixScan_LeetCodeExamples_ReturnsPerQueryMaxSplitPrimeCounts(
        int[] nums, int[][] queries, int[] expected) =>
        Assert.Equal(
            expected,
            MaximizeCountOfDistinctPrimesAfterSplitSolution.MaxDistinctPrimeCountsByPrefixSuffixScan(nums, queries));
}
