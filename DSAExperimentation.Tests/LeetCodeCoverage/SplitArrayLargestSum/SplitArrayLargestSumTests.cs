using DSAExperimentation.LeetCode.SplitArrayLargestSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SplitArrayLargestSum;

// Harness only. Both strategies are SplitArrayLargestSumSolution's - this file
// just pins them to LeetCode's published examples, plus the k=1 and
// k=nums.Length boundaries neither original arm exercised.
public sealed class SplitArrayLargestSumTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [7, 2, 5, 10, 8], 2, 18 },
            { [1, 2, 3, 4, 5], 2, 9 },
            { [1, 4, 4], 3, 4 },
            { [1, 2, 3, 4, 5], 1, 15 },
            { [1, 2, 3, 4, 5], 5, 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimizedLargestSumByManualBinarySearch_LeetCodeExamples_ReturnsSmallestFeasibleMax(
        int[] nums, int k, int expected)
    {
        var actual = SplitArrayLargestSumSolution.MinimizedLargestSumByManualBinarySearch(nums, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimizedLargestSumBySequenceLowerBound_LeetCodeExamples_ReturnsSmallestFeasibleMax(
        int[] nums, int k, int expected)
    {
        var actual = SplitArrayLargestSumSolution.MinimizedLargestSumBySequenceLowerBound(nums, k);

        Assert.Equal(expected, actual);
    }
}
