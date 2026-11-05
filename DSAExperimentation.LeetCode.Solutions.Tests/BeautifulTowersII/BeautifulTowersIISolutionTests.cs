using DSAExperimentation.LeetCode.Solutions.BeautifulTowersII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BeautifulTowersII;

// Harness only. Both strategies live in BeautifulTowersIISolution, which reuses
// BeautifulTowersISolution's - LC 2866 is LC 2865 at a larger bound. This file
// pins them to the shape II's own scale is about: 1,000 towers all at the 1e9 cap,
// whose 1e12 answer overflows int and so proves the long accumulation is real
// rather than incidental. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class BeautifulTowersIISolutionTests
{
    private const int MaxHeight = 1_000_000_000;
    private const int FlatTowerCount = 1_000;

    public static TheoryData<int[], long> Cases =>
        new()
        {
            { [.. Enumerable.Repeat(MaxHeight, FlatTowerCount)], (long)MaxHeight * FlatTowerCount },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumSumOfHeightsByBruteForce_Cases_ReturnsMaxSum(
        int[] maxHeights, long expected) =>
        Assert.Equal(expected, BeautifulTowersIISolution.MaximumSumOfHeightsByBruteForce(maxHeights));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumSumOfHeightsByMonotonicStack_Cases_ReturnsMaxSum(
        int[] maxHeights, long expected) =>
        Assert.Equal(expected, BeautifulTowersIISolution.MaximumSumOfHeightsByMonotonicStack(maxHeights));
}
