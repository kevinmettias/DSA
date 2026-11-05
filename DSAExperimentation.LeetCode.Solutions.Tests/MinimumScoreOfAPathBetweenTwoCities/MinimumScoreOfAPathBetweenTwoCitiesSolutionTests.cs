using DSAExperimentation.LeetCode.Solutions.MinimumScoreOfAPathBetweenTwoCities;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumScoreOfAPathBetweenTwoCities;

// Harness only. Both strategies are MinimumScoreOfAPathBetweenTwoCitiesSolution's -
// including the flood fill, which the benchmark used to own privately as its baseline
// and nothing asserted.
public sealed partial class MinimumScoreOfAPathBetweenTwoCitiesSolutionTests
{
    public static TheoryData<int, int[][], int> Cases =>
        new()
        {
            // A cheaper road in a component city 1 cannot reach must be ignored.
            { 6, [[1, 2, 9], [2, 3, 6], [2, 4, 5], [1, 4, 7], [5, 6, 1]], 5 },

            // The minimum sits two hops away, so an arm that only inspects city 1's
            // own roads is wrong.
            { 4, [[1, 2, 5], [2, 3, 1], [3, 4, 4]], 1 },

            // The smallest input LC allows: one road, and it is the answer.
            { 2, [[1, 2, 3]], 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinScoreByBreadthFirstFloodFill_Cases_ReturnsMinimumEdgeWeightInCity1Component(
        int cityCount, int[][] roads, int expected)
    {
        var actual = MinimumScoreOfAPathBetweenTwoCitiesSolution.MinScoreByBreadthFirstFloodFill(cityCount, roads);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinScoreByDisjointSet_Cases_ReturnsMinimumEdgeWeightInCity1Component(
        int cityCount, int[][] roads, int expected)
    {
        var actual = MinimumScoreOfAPathBetweenTwoCitiesSolution.MinScoreByDisjointSet(cityCount, roads);
        Assert.Equal(expected, actual);
    }
}
