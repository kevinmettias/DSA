using DSAExperimentation.LeetCode.CountSubtreesWithMaxDistanceBetweenCities;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSubtreesWithMaxDistanceBetweenCities;

// Harness only. Both strategies are
// CountSubtreesWithMaxDistanceBetweenCitiesSolution's - this file just pins them to
// LeetCode's three published examples plus a four-city path, whose buckets separate
// a chain from the star shape example 1 already covers.
public sealed partial class CountSubtreesWithMaxDistanceBetweenCitiesTests
{
    public static TheoryData<int, int[][], int[]> Examples =>
        new()
        {
            { 4, [[1, 2], [2, 3], [2, 4]], [3, 4, 0] },
            { 2, [[1, 2]], [1] },
            { 3, [[1, 2], [2, 3]], [2, 1] },
            { 4, [[1, 2], [2, 3], [3, 4]], [3, 2, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountSubtreesByAllPairsBfs_LeetCodeExamples_ReturnsSubtreeCountPerMaxDistance(
        int cityCount, int[][] edges, int[] expected)
    {
        var distances = CountSubtreesWithMaxDistanceBetweenCitiesSolution.CountSubtreesByAllPairsBfs(cityCount, edges);
        Assert.Equal(expected, distances);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountSubtreesByDoubleBfs_LeetCodeExamples_ReturnsSubtreeCountPerMaxDistance(
        int cityCount, int[][] edges, int[] expected)
    {
        var distances = CountSubtreesWithMaxDistanceBetweenCitiesSolution.CountSubtreesByDoubleBfs(cityCount, edges);
        Assert.Equal(expected, distances);
    }
}
