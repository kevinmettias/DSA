using DSAExperimentation.LeetCode.NumberOfWaysToArriveAtDestination;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToArriveAtDestination;

// Harness only. The graph, its Dijkstra distance labelling and both counting
// strategies are NumberOfWaysToArriveAtDestinationSolution's - this file just pins
// them to LeetCode's published examples, plus the diamond where two equal-time
// routes converge and the one-intersection graph where the journey is already over.
public sealed partial class NumberOfWaysToArriveAtDestinationTests
{
    public static TheoryData<int, int[][], long> Examples =>
        new()
        {
            {
                7,
                [
                    [0, 6, 7], [0, 1, 2], [1, 2, 3], [1, 3, 3], [6, 3, 3],
                    [3, 5, 1], [6, 5, 1], [2, 5, 1], [0, 4, 5], [4, 6, 2],
                ],
                4
            },
            { 2, [[1, 0, 10]], 1 },
            { 2, [[0, 1, 1]], 1 },
            { 4, [[0, 1, 1], [0, 2, 1], [1, 3, 1], [2, 3, 1]], 2 },
            { 4, [[0, 1, 1], [0, 2, 5], [1, 3, 1], [2, 3, 1]], 1 },
            { 1, [], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByNaiveDfs_LeetCodeExamples_CountsEveryShortestTimeJourney(
        int intersectionCount, int[][] roads, long expected)
    {
        var actual = NumberOfWaysToArriveAtDestinationSolution.CountWaysByNaiveDfs(intersectionCount, roads);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByDagFold_LeetCodeExamples_CountsEveryShortestTimeJourney(
        int intersectionCount, int[][] roads, long expected)
    {
        var actual = NumberOfWaysToArriveAtDestinationSolution.CountWaysByDagFold(intersectionCount, roads);

        Assert.Equal(expected, actual);
    }
}
