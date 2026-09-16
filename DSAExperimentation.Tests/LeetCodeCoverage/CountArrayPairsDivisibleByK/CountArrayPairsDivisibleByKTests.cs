using DSAExperimentation.LeetCode.CountArrayPairsDivisibleByK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountArrayPairsDivisibleByK;

// Harness only. Both the pairwise scan and the HashMap gcd-grouping are
// CountArrayPairsDivisibleByKSolution's; this file pins them to LeetCode's published
// examples plus the cases the grouping has to get right on its own - every value
// landing in one group, divisor = 1 so every pair qualifies, and a mix where the qualifying
// pairs all straddle two different groups.
public sealed partial class CountArrayPairsDivisibleByKTests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], 2, 7L },
            { [1, 2, 3, 4], 5, 0L },
            { [2, 2, 2, 2], 2, 6L },
            { [3, 5, 7], 1, 3L },
            { [1, 2, 3, 4, 5], 1, 10L },
            { [4, 7, 9, 14, 21], 7, 9L },
            { [5], 5, 0L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByBruteForce_LeetCodeExamples_ReturnsDivisiblePairCount(
        int[] nums, int divisor, long expected)
    {
        var actual = CountArrayPairsDivisibleByKSolution.CountPairsByBruteForce(nums, divisor);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByGcdGroups_LeetCodeExamples_ReturnsDivisiblePairCount(
        int[] nums, int divisor, long expected)
    {
        var actual = CountArrayPairsDivisibleByKSolution.CountPairsByGcdGroups(nums, divisor);

        Assert.Equal(expected, actual);
    }
}
