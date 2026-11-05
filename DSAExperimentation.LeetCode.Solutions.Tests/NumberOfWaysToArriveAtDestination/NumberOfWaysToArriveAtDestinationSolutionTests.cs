using DSAExperimentation.LeetCode.Solutions.NumberOfWaysToArriveAtDestination;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfWaysToArriveAtDestination;

// Harness only. The graph, its Dijkstra distance labelling and both counting
// strategies are NumberOfWaysToArriveAtDestinationSolution's - this file just pins
// them to the diamond where two equal-time
// routes converge and the one-intersection graph where the journey is already over.
public sealed partial class NumberOfWaysToArriveAtDestinationSolutionTests
{
    public static TheoryData<int, int[][], long> Cases =>
        new()
        {
            { 2, [[0, 1, 1]], 1 },
            { 4, [[0, 1, 1], [0, 2, 1], [1, 3, 1], [2, 3, 1]], 2 },
            { 4, [[0, 1, 1], [0, 2, 5], [1, 3, 1], [2, 3, 1]], 1 },
            { 1, [], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountWaysByNaiveDfs_Cases_CountsEveryShortestTimeJourney(
        int intersectionCount, int[][] roads, long expected)
    {
        var actual = NumberOfWaysToArriveAtDestinationSolution.CountWaysByNaiveDfs(intersectionCount, roads);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountWaysByDagFold_Cases_CountsEveryShortestTimeJourney(
        int intersectionCount, int[][] roads, long expected)
    {
        var actual = NumberOfWaysToArriveAtDestinationSolution.CountWaysByDagFold(intersectionCount, roads);

        Assert.Equal(expected, actual);
    }
}
