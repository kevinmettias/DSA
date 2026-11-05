using DSAExperimentation.LeetCode.Solutions.SecondMinimumTimeToReachDestination;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SecondMinimumTimeToReachDestination;

// Harness only. Both dual-distance walks are
// SecondMinimumTimeToReachDestinationSolution's - this file pins them to the cases
// LeetCode's examples never reach: a trip short enough that no signal is ever red, a
// bipartite graph where the second-shortest route is two roads longer rather than
// one, a triangle where it is exactly one longer, and a signal so short that every
// single arrival waits. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class SecondMinimumTimeToReachDestinationSolutionTests
{
    public static TheoryData<ArrivalCase> Cases =>
        new()
        {
            // The same graph as LeetCode example 1 with a signal that never turns red
            // inside the trip: three roads at three minutes each.
            new ArrivalCase(
                NodeCount: 5, Edges: [[1, 2], [1, 3], [1, 4], [3, 4], [4, 5]], Time: 3, Change: 100, Expected: 9),

            // A path graph is bipartite, so every route to the far end uses an odd
            // number of roads - the second minimum is two more, not one.
            new ArrivalCase(NodeCount: 3, Edges: [[1, 2], [2, 3]], Time: 1, Change: 100, Expected: 4),

            // A triangle is not bipartite: one road there directly, two the long way.
            new ArrivalCase(NodeCount: 3, Edges: [[1, 2], [2, 3], [1, 3]], Time: 1, Change: 100, Expected: 2),

            // A star: out to a leaf, back to the center, then on to the destination.
            new ArrivalCase(NodeCount: 4, Edges: [[1, 2], [1, 3], [1, 4]], Time: 1, Change: 100, Expected: 3),

            // A four-node path with a signal shorter than two crossings, so two of the
            // five arrivals wait out a red light.
            new ArrivalCase(NodeCount: 4, Edges: [[1, 2], [2, 3], [3, 4]], Time: 2, Change: 3, Expected: 14),

            // The shortest possible signal: every arrival after the first lands on red.
            new ArrivalCase(NodeCount: 2, Edges: [[1, 2]], Time: 1, Change: 1, Expected: 5),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SecondMinimumTimeByListFrontier_Cases_ReturnsSecondShortestArrivalTime(
        ArrivalCase testCase)
    {
        var minutes = SecondMinimumTimeToReachDestinationSolution.SecondMinimumTimeByListFrontier(
            testCase.NodeCount, testCase.Edges, testCase.Time, testCase.Change);

        Assert.Equal(testCase.Expected, minutes);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SecondMinimumTimeByQueueFrontier_Cases_ReturnsSecondShortestArrivalTime(
        ArrivalCase testCase)
    {
        var minutes = SecondMinimumTimeToReachDestinationSolution.SecondMinimumTimeByQueueFrontier(
            testCase.NodeCount, testCase.Edges, testCase.Time, testCase.Change);

        Assert.Equal(testCase.Expected, minutes);
    }

    // One case: the node count, the roads with their crossing times, the
    // signal's green duration, how long each light stays red, and the second-shortest
    // arrival time. The five are one case, so the signature carries one parameter
    // rather than five positions.
    public readonly record struct ArrivalCase(
        int NodeCount,
        int[][] Edges,
        int Time,
        int Change,
        int Expected);
}
