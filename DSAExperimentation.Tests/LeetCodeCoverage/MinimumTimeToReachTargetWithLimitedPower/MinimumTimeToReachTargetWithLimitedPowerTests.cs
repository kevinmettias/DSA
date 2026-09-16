using DSAExperimentation.LeetCode.MinimumTimeToReachTargetWithLimitedPower;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumTimeToReachTargetWithLimitedPower;

// Harness only. PowerStateGraph is
// MinimumTimeToReachTargetWithLimitedPowerSolution's own domain model and both
// search strategies are its methods - this file just pins them to LeetCode's
// published examples, including the same-node case (no traversal needed, full
// power reported back) and the unreachable case.
public sealed class MinimumTimeToReachTargetWithLimitedPowerTests
{
    public static TheoryData<PowerExample> Examples =>
        new()
        {
            {
                new PowerExample(
                    N: 5,
                    Edges: [[0, 1, 1], [1, 4, 1], [0, 2, 1], [2, 3, 1], [3, 4, 1]],
                    Power: 4,
                    Cost: [2, 3, 1, 1, 1],
                    Source: 0,
                    Target: 4,
                    Expected: [3L, 0L])
            },
            {
                new PowerExample(
                    N: 3,
                    Edges: [[0, 1, 2], [1, 2, 2], [2, 0, 2]],
                    Power: 3,
                    Cost: [1, 1, 1],
                    Source: 1,
                    Target: 1,
                    Expected: [0L, 3L])
            },
            {
                new PowerExample(
                    N: 4,
                    Edges: [[0, 1, 3], [2, 3, 4]],
                    Power: 3,
                    Cost: [1, 1, 1, 1],
                    Source: 0,
                    Target: 3,
                    Expected: [-1L, -1L])
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeMaxPowerByBclPriorityQueue_LeetCodeExamples_ReturnsTimeAndRemainingPower(
        PowerExample example)
    {
        var actual = MinimumTimeToReachTargetWithLimitedPowerSolution.MinTimeMaxPowerByBclPriorityQueue(
            (example.N, example.Edges, example.Power, example.Cost), example.Source, example.Target);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeMaxPowerByReduceGraph_LeetCodeExamples_ReturnsTimeAndRemainingPower(
        PowerExample example)
    {
        var actual = MinimumTimeToReachTargetWithLimitedPowerSolution.MinTimeMaxPowerByReduceGraph(
            (example.N, example.Edges, example.Power, example.Cost), example.Source, example.Target);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the edge list, the power budget each node charges to
    // enter, the budget the walk starts with, the requested endpoints, and the
    // (time, power left) pair each strategy has to agree on. The three counts are all
    // `int` and the graph is four more values, so the fields name each one rather than
    // leaving a row of bare numbers a caller could not place.
    public readonly record struct PowerExample(
        int N,
        int[][] Edges,
        int Power,
        int[] Cost,
        int Source,
        int Target,
        long[] Expected);
}
