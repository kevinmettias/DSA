using DSAExperimentation.LeetCode.MinimumScoreOfAPathBetweenTwoCities;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumScoreOfAPathBetweenTwoCities;

// Harness only. Both strategies are MinimumScoreOfAPathBetweenTwoCitiesSolution's -
// including the flood fill, which the benchmark used to own privately as its baseline
// and nothing asserted.
public sealed class MinimumScoreOfAPathBetweenTwoCitiesTests
{
    public static TheoryData<int, int[][], int> Examples =>
        new()
        {
            // LC example 1: the answer is the [2,4] road, which lies on no shortest
            // 1 -> 4 route - the detour onto it is free because roads may repeat.
            { 4, [[1, 2, 9], [2, 3, 6], [2, 4, 5], [1, 4, 7]], 5 },

            // LC example 2.
            { 4, [[1, 2, 2], [1, 3, 4], [3, 4, 7]], 2 },

            // A cheaper road in a component city 1 cannot reach must be ignored.
            { 6, [[1, 2, 9], [2, 3, 6], [2, 4, 5], [1, 4, 7], [5, 6, 1]], 5 },

            // The minimum sits two hops away, so an arm that only inspects city 1's
            // own roads is wrong.
            { 4, [[1, 2, 5], [2, 3, 1], [3, 4, 4]], 1 },

            // The smallest input LC allows: one road, and it is the answer.
            { 2, [[1, 2, 3]], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinScoreByBreadthFirstFloodFill_LeetCodeExamples_ReturnsMinimumEdgeWeightInCity1Component(
        int cityCount, int[][] roads, int expected) =>
        Assert.Equal(
            expected,
            MinimumScoreOfAPathBetweenTwoCitiesSolution.MinScoreByBreadthFirstFloodFill(cityCount, roads));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinScoreByDisjointSet_LeetCodeExamples_ReturnsMinimumEdgeWeightInCity1Component(
        int cityCount, int[][] roads, int expected) =>
        Assert.Equal(
            expected,
            MinimumScoreOfAPathBetweenTwoCitiesSolution.MinScoreByDisjointSet(cityCount, roads));
}
