using DSAExperimentation.LeetCode.FindTheCountOfMonotonicPairsI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheCountOfMonotonicPairsI;

// Harness only. Both strategies are FindTheCountOfMonotonicPairsISolution's -
// this file just pins them to LeetCode's published examples.
public sealed partial class FindTheCountOfMonotonicPairsITests
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
        Assert.Equal(expected, FindTheCountOfMonotonicPairsISolution.CountPairsByBruteForceDP(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByPrefixSumDP_LeetCodeExamples_ReturnsMonotonicPairCountModulo(
        int[] nums, long expected) =>
        Assert.Equal(expected, FindTheCountOfMonotonicPairsISolution.CountPairsByPrefixSumDP(nums));
}
