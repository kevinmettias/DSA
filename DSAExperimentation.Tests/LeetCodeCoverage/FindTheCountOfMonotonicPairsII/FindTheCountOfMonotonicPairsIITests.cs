using DSAExperimentation.LeetCode.FindTheCountOfMonotonicPairsII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheCountOfMonotonicPairsII;

// Harness only. Both strategies are FindTheCountOfMonotonicPairsIISolution's -
// this file just pins them to LeetCode's published examples (the same examples
// Part I publishes, since the two problems share one statement and differ only
// in how large nums[i] is allowed to be).
public sealed class FindTheCountOfMonotonicPairsIITests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [2, 3, 2], 4 },
            { [5, 5, 5, 5], 126 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByBruteForceDP_LeetCodeExamples_ReturnsMonotonicPairCountModulo(
        int[] nums, long expected) =>
        Assert.Equal(expected, FindTheCountOfMonotonicPairsIISolution.CountPairsByBruteForceDP(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByPrefixSumDP_LeetCodeExamples_ReturnsMonotonicPairCountModulo(
        int[] nums, long expected) =>
        Assert.Equal(expected, FindTheCountOfMonotonicPairsIISolution.CountPairsByPrefixSumDP(nums));
}
