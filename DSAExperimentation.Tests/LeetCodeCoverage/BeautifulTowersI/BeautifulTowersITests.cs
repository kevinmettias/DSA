using DSAExperimentation.LeetCode.BeautifulTowersI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BeautifulTowersI;

// Harness only. Both strategies live in BeautifulTowersISolution - this file pins
// them to LeetCode's three published examples plus a single tower (nothing to
// clamp on either side of the peak), a strictly increasing array (the peak is the
// last index and the whole array is its own left run) and a flat array (every
// tower keeps its cap).
public sealed class BeautifulTowersITests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [5, 3, 4, 1, 1], 13 },
            { [6, 5, 3, 9, 2, 7], 22 },
            { [3, 2, 5, 5, 2, 3], 18 },
            { [7], 7 },
            { [1, 2, 3, 4, 5], 15 },
            { [5, 4, 3, 2, 1], 15 },
            { [4, 4, 4, 4], 16 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumOfHeightsByBruteForce_LeetCodeExamples_ReturnsMaxSum(
        int[] maxHeights, long expected) =>
        Assert.Equal(expected, BeautifulTowersISolution.MaximumSumOfHeightsByBruteForce(maxHeights));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumOfHeightsByMonotonicStack_LeetCodeExamples_ReturnsMaxSum(
        int[] maxHeights, long expected) =>
        Assert.Equal(expected, BeautifulTowersISolution.MaximumSumOfHeightsByMonotonicStack(maxHeights));
}
