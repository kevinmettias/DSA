using DSAExperimentation.LeetCode.Solutions.CountArrayPairsDivisibleByK;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountArrayPairsDivisibleByK;

// Harness only. Both the pairwise scan and the HashMap gcd-grouping are
// CountArrayPairsDivisibleByKSolution's; this file pins them to the cases the
// grouping has to get right on its own - every value landing in one group,
// divisor = 1 so every pair qualifies, a mix whose qualifying pairs come both from
// inside one group and from across two, and a single value with no pair at all.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class CountArrayPairsDivisibleByKSolutionTests
{
    public static TheoryData<int[], int, long> Cases =>
        new()
        {
            { [2, 2, 2, 2], 2, 6L },
            { [3, 5, 7], 1, 3L },
            { [1, 2, 3, 4, 5], 1, 10L },
            { [4, 7, 9, 14, 21], 7, 9L },
            { [5], 5, 0L },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountPairsByBruteForce_Cases_ReturnsDivisiblePairCount(
        int[] nums, int divisor, long expected)
    {
        var actual = CountArrayPairsDivisibleByKSolution.CountPairsByBruteForce(nums, divisor);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountPairsByGcdGroups_Cases_ReturnsDivisiblePairCount(
        int[] nums, int divisor, long expected)
    {
        var actual = CountArrayPairsDivisibleByKSolution.CountPairsByGcdGroups(nums, divisor);

        Assert.Equal(expected, actual);
    }
}
