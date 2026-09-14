using DSAExperimentation.LeetCode.BeautifulTowersII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BeautifulTowersII;

// Harness only. Both strategies live in BeautifulTowersIISolution, which reuses
// BeautifulTowersISolution's - LC 2866 is LC 2865 at a larger bound. This file
// pins them to the same three published examples plus the two shapes II's own
// scale is about: a strictly increasing array (peak at the last index) and 1,000
// towers all at the 1e9 cap, whose 1e12 answer overflows int and so proves the
// long accumulation is real rather than incidental.
public sealed class BeautifulTowersIITests
{
    private const int MaxHeight = 1_000_000_000;
    private const int FlatTowerCount = 1_000;

    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [5, 3, 4, 1, 1], 13 },
            { [6, 5, 3, 9, 2, 7], 22 },
            { [3, 2, 5, 5, 2, 3], 18 },
            { [1, 2, 3, 4, 5, 6, 7, 8, 9, 10], 55 },
            { [.. Enumerable.Repeat(MaxHeight, FlatTowerCount)], (long)MaxHeight * FlatTowerCount },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumOfHeightsByBruteForce_LeetCodeExamples_ReturnsMaxSum(
        int[] maxHeights, long expected) =>
        Assert.Equal(expected, BeautifulTowersIISolution.MaximumSumOfHeightsByBruteForce(maxHeights));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumOfHeightsByMonotonicStack_LeetCodeExamples_ReturnsMaxSum(
        int[] maxHeights, long expected) =>
        Assert.Equal(expected, BeautifulTowersIISolution.MaximumSumOfHeightsByMonotonicStack(maxHeights));
}
