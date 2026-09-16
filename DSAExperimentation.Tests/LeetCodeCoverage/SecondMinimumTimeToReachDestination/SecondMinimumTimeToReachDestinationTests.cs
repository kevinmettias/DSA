using DSAExperimentation.LeetCode.SecondMinimumTimeToReachDestination;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SecondMinimumTimeToReachDestination;

// Harness only. Both dual-distance walks are
// SecondMinimumTimeToReachDestinationSolution's - this file pins them to LeetCode's
// two published examples plus the cases the examples never reach: a trip short enough
// that no signal is ever red, a bipartite graph where the second-shortest route is two
// roads longer rather than one, a triangle where it is exactly one longer, and a
// signal so short that every single arrival waits.
public sealed class SecondMinimumTimeToReachDestinationTests
{
    public static TheoryData<ArrivalExample> Examples =>
        new()
        {
            // LeetCode example 1: the two-road route takes 6 minutes, so the answer is
            // the three-road one - which waits out one red light on the way.
            new ArrivalExample(
                NodeCount: 5, Edges: [[1, 2], [1, 3], [1, 4], [3, 4], [4, 5]], Time: 3, Change: 5, Expected: 13),

            // LeetCode example 2: the only road forces a trip back and forth, three
            // crossings in all, two of them starting on red.
            new ArrivalExample(NodeCount: 2, Edges: [[1, 2]], Time: 3, Change: 2, Expected: 11),

            // The same graph as example 1 with a signal that never turns red inside the
            // trip: three roads at three minutes each.
            new ArrivalExample(
                NodeCount: 5, Edges: [[1, 2], [1, 3], [1, 4], [3, 4], [4, 5]], Time: 3, Change: 100, Expected: 9),

            // A path graph is bipartite, so every route to the far end uses an odd
            // number of roads - the second minimum is two more, not one.
            new ArrivalExample(NodeCount: 3, Edges: [[1, 2], [2, 3]], Time: 1, Change: 100, Expected: 4),

            // A triangle is not bipartite: one road there directly, two the long way.
            new ArrivalExample(NodeCount: 3, Edges: [[1, 2], [2, 3], [1, 3]], Time: 1, Change: 100, Expected: 2),

            // A star: out to a leaf, back to the center, then on to the destination.
            new ArrivalExample(NodeCount: 4, Edges: [[1, 2], [1, 3], [1, 4]], Time: 1, Change: 100, Expected: 3),

            // A four-node path with a signal shorter than two crossings, so two of the
            // five arrivals wait out a red light.
            new ArrivalExample(NodeCount: 4, Edges: [[1, 2], [2, 3], [3, 4]], Time: 2, Change: 3, Expected: 14),

            // The shortest possible signal: every arrival after the first lands on red.
            new ArrivalExample(NodeCount: 2, Edges: [[1, 2]], Time: 1, Change: 1, Expected: 5),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SecondMinimumTimeByListFrontier_LeetCodeExamples_ReturnsSecondShortestArrivalTime(
        ArrivalExample example)
    {
        var minutes = SecondMinimumTimeToReachDestinationSolution.SecondMinimumTimeByListFrontier(
            example.NodeCount, example.Edges, example.Time, example.Change);

        Assert.Equal(example.Expected, minutes);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SecondMinimumTimeByQueueFrontier_LeetCodeExamples_ReturnsSecondShortestArrivalTime(
        ArrivalExample example)
    {
        var minutes = SecondMinimumTimeToReachDestinationSolution.SecondMinimumTimeByQueueFrontier(
            example.NodeCount, example.Edges, example.Time, example.Change);

        Assert.Equal(example.Expected, minutes);
    }

    // One LeetCode example: the node count, the roads with their crossing times, the
    // signal's green duration, how long each light stays red, and the second-shortest
    // arrival time. The five are one case, so the signature carries one parameter
    // rather than five positions.
    public readonly record struct ArrivalExample(
        int NodeCount,
        int[][] Edges,
        int Time,
        int Change,
        int Expected);
}
