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
    public static TheoryData<int, int[][], int, int, int> Examples =>
        new()
        {
            // LeetCode example 1: the two-road route takes 6 minutes, so the answer is
            // the three-road one - which waits out one red light on the way.
            { 5, [[1, 2], [1, 3], [1, 4], [3, 4], [4, 5]], 3, 5, 13 },

            // LeetCode example 2: the only road forces a trip back and forth, three
            // crossings in all, two of them starting on red.
            { 2, [[1, 2]], 3, 2, 11 },

            // The same graph as example 1 with a signal that never turns red inside the
            // trip: three roads at three minutes each.
            { 5, [[1, 2], [1, 3], [1, 4], [3, 4], [4, 5]], 3, 100, 9 },

            // A path graph is bipartite, so every route to the far end uses an odd
            // number of roads - the second minimum is two more, not one.
            { 3, [[1, 2], [2, 3]], 1, 100, 4 },

            // A triangle is not bipartite: one road there directly, two the long way.
            { 3, [[1, 2], [2, 3], [1, 3]], 1, 100, 2 },

            // A star: out to a leaf, back to the center, then on to the destination.
            { 4, [[1, 2], [1, 3], [1, 4]], 1, 100, 3 },

            // A four-node path with a signal shorter than two crossings, so two of the
            // five arrivals wait out a red light.
            { 4, [[1, 2], [2, 3], [3, 4]], 2, 3, 14 },

            // The shortest possible signal: every arrival after the first lands on red.
            { 2, [[1, 2]], 1, 1, 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SecondMinimumTimeByListFrontier_LeetCodeExamples_ReturnsSecondShortestArrivalTime(
        int n, int[][] edges, int time, int change, int expected)
    {
        var minutes = SecondMinimumTimeToReachDestinationSolution.SecondMinimumTimeByListFrontier(n, edges, time, change);

        Assert.Equal(expected, minutes);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SecondMinimumTimeByQueueFrontier_LeetCodeExamples_ReturnsSecondShortestArrivalTime(
        int n, int[][] edges, int time, int change, int expected)
    {
        var minutes = SecondMinimumTimeToReachDestinationSolution.SecondMinimumTimeByQueueFrontier(n, edges, time, change);

        Assert.Equal(expected, minutes);
    }
}
