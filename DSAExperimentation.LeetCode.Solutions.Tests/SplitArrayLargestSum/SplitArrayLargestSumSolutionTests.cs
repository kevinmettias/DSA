using DSAExperimentation.LeetCode.Solutions.SplitArrayLargestSum;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SplitArrayLargestSum;

// Harness only. Both strategies are SplitArrayLargestSumSolution's - this file
// just pins them to the subarrayCount = 1 and
// subarrayCount = nums.Length boundaries neither original arm exercised.
public sealed partial class SplitArrayLargestSumSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [1, 2, 3, 4, 5], 1, 15 },
            { [1, 2, 3, 4, 5], 5, 5 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimizedLargestSumByManualBinarySearch_Cases_ReturnsSmallestFeasibleMax(
        int[] nums, int subarrayCount, int expected)
    {
        var actual = SplitArrayLargestSumSolution.MinimizedLargestSumByManualBinarySearch(nums, subarrayCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimizedLargestSumByPredicateSearch_Cases_ReturnsSmallestFeasibleMax(
        int[] nums, int subarrayCount, int expected)
    {
        var actual = SplitArrayLargestSumSolution.MinimizedLargestSumByPredicateSearch(nums, subarrayCount);

        Assert.Equal(expected, actual);
    }
}
