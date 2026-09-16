using DSAExperimentation.LeetCode.RobotCollisions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RobotCollisions;

// Harness only: both strategies are RobotCollisionsSolution's, the same
// restart-the-scan baseline and single-sweep stack simulation
// RobotCollisionsBenchmarks measures.
public sealed partial class RobotCollisionsTests
{
    public static TheoryData<int[], int[], string, int[]> Examples =>
        new()
        {
            // LeetCode example 1: everything moves right, so nothing ever collides.
            { [5, 4, 3, 2, 1], [2, 17, 9, 15, 10], "RRRRR", [2, 17, 9, 15, 10] },

            // LeetCode example 2: the two equal-health robots destroy each other,
            // then the strongest right-mover survives its collision with one health lost.
            { [3, 5, 2, 6], [10, 10, 15, 12], "RLRL", [14] },

            // LeetCode example 3: both collisions are ties, so no robot survives.
            { [1, 2, 5, 6], [10, 10, 11, 11], "RLRL", [] },

            // One left-mover cascading through three right-movers, losing one health per kill.
            { [1, 2, 3, 4], [5, 3, 1, 10], "RRRL", [7] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SurvivorHealthsByRepeatedScan_LeetCodeExamples_ReturnsSurvivingHealthsInInputOrder(
        int[] positions, int[] healths, string directions, int[] expected)
    {
        var actual = RobotCollisionsSolution.SurvivorHealthsByRepeatedScan(positions, healths, directions);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SurvivorHealthsByStackSimulation_LeetCodeExamples_ReturnsSurvivingHealthsInInputOrder(
        int[] positions, int[] healths, string directions, int[] expected)
    {
        var actual = RobotCollisionsSolution.SurvivorHealthsByStackSimulation(positions, healths, directions);

        Assert.Equal(expected, actual);
    }
}
