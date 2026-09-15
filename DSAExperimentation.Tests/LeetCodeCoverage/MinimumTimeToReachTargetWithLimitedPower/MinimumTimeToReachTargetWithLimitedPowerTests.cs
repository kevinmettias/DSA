using DSAExperimentation.LeetCode.MinimumTimeToReachTargetWithLimitedPower;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumTimeToReachTargetWithLimitedPower;

// Harness only. PowerStateGraph is
// MinimumTimeToReachTargetWithLimitedPowerSolution's own domain model and both
// search strategies are its methods - this file just pins them to LeetCode's
// published examples, including the same-node case (no traversal needed, full
// power reported back) and the unreachable case.
public sealed class MinimumTimeToReachTargetWithLimitedPowerTests
{
    public static TheoryData<int, int[][], int, int[], int, int, long[]> Examples =>
        new()
        {
            {
                5,
                [[0, 1, 1], [1, 4, 1], [0, 2, 1], [2, 3, 1], [3, 4, 1]],
                4,
                [2, 3, 1, 1, 1],
                0,
                4,
                [3L, 0L]
            },
            {
                3,
                [[0, 1, 2], [1, 2, 2], [2, 0, 2]],
                3,
                [1, 1, 1],
                1,
                1,
                [0L, 3L]
            },
            {
                4,
                [[0, 1, 3], [2, 3, 4]],
                3,
                [1, 1, 1, 1],
                0,
                3,
                [-1L, -1L]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeMaxPowerByBclPriorityQueue_LeetCodeExamples_ReturnsTimeAndRemainingPower(
        int n, int[][] edges, int power, int[] cost, int source, int target, long[] expected) =>
        Assert.Equal(
            expected,
            MinimumTimeToReachTargetWithLimitedPowerSolution.MinTimeMaxPowerByBclPriorityQueue(
                (n, edges, power, cost), source, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeMaxPowerByReduceGraph_LeetCodeExamples_ReturnsTimeAndRemainingPower(
        int n, int[][] edges, int power, int[] cost, int source, int target, long[] expected) =>
        Assert.Equal(
            expected,
            MinimumTimeToReachTargetWithLimitedPowerSolution.MinTimeMaxPowerByReduceGraph(
                (n, edges, power, cost), source, target));
}
