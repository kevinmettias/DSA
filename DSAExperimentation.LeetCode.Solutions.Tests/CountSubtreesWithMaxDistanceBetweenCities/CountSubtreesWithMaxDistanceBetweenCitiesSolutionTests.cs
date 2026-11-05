using DSAExperimentation.LeetCode.Solutions.CountSubtreesWithMaxDistanceBetweenCities;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountSubtreesWithMaxDistanceBetweenCities;

// Harness only. Both strategies are
// CountSubtreesWithMaxDistanceBetweenCitiesSolution's - this file just pins them to
// a four-city path, whose buckets separate a chain from the star shape of
// LeetCode's first example. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class CountSubtreesWithMaxDistanceBetweenCitiesSolutionTests
{
    public static TheoryData<int, int[][], int[]> Cases =>
        new()
        {
            { 4, [[1, 2], [2, 3], [3, 4]], [3, 2, 1] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountSubtreesByAllPairsBfs_Cases_ReturnsSubtreeCountPerMaxDistance(
        int cityCount, int[][] edges, int[] expected)
    {
        var distances = CountSubtreesWithMaxDistanceBetweenCitiesSolution.CountSubtreesByAllPairsBfs(cityCount, edges);
        Assert.Equal(expected, distances);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountSubtreesByDoubleBfs_Cases_ReturnsSubtreeCountPerMaxDistance(
        int cityCount, int[][] edges, int[] expected)
    {
        var distances = CountSubtreesWithMaxDistanceBetweenCitiesSolution.CountSubtreesByDoubleBfs(cityCount, edges);
        Assert.Equal(expected, distances);
    }
}
