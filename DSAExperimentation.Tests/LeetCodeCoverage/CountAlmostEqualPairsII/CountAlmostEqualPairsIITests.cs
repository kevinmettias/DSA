using DSAExperimentation.LeetCode.CountAlmostEqualPairsII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountAlmostEqualPairsII;

// Harness only: both strategies live in CountAlmostEqualPairsIISolution - this file
// just pins them to LeetCode's published examples.
public sealed partial class CountAlmostEqualPairsIITests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [1023, 2310, 2130, 213], 4 },
            { [1, 10, 100], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBoundedSwapBruteForce_LeetCodeExamples_ReturnsAlmostEqualPairCount(int[] nums, long expected) =>
        Assert.Equal(expected, CountAlmostEqualPairsIISolution.CountByBoundedSwapBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBoundedSwapBacktrack_LeetCodeExamples_ReturnsAlmostEqualPairCount(int[] nums, long expected) =>
        Assert.Equal(expected, CountAlmostEqualPairsIISolution.CountByBoundedSwapBacktrack(nums));
}
