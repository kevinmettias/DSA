using DSAExperimentation.LeetCode.SmallestUniqueSubarray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestUniqueSubarray;

// Harness only: both strategies live in SmallestUniqueSubarraySolution. One test
// method per strategy over one shared set of LeetCode's own examples, so a failure
// names the strategy that broke.
public sealed class SmallestUniqueSubarrayTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [3, 3, 3], 3 },
            { [2, 1, 2, 3, 3], 1 },
            { [1, 1, 2, 2, 1], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestUniqueLengthByBruteForce_LeetCodeExamples_ReturnsShortestUniqueLength(
        int[] nums, int expected)
    {
        var actual = SmallestUniqueSubarraySolution.SmallestUniqueLengthByBruteForce(nums);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestUniqueLengthByRollingHash_LeetCodeExamples_ReturnsShortestUniqueLength(
        int[] nums, int expected)
    {
        var actual = SmallestUniqueSubarraySolution.SmallestUniqueLengthByRollingHash(nums);

        Assert.Equal(expected, actual);
    }
}
