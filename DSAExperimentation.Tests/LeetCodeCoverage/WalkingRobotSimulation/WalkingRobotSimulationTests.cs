using DSAExperimentation.LeetCode.WalkingRobotSimulation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WalkingRobotSimulation;

// Harness only. Both strategies are WalkingRobotSimulationSolution's - the linear
// obstacle scan that used to live only in the benchmark's baseline arm, and the
// Set<(int, int)> walk the test used to inline.
public sealed partial class WalkingRobotSimulationTests
{
    public static TheoryData<int[], int[][], int> Examples =>
        new()
        {
            // LC example 1: no obstacles, so both walks run to completion.
            { [4, -1, 3], [], 25 },

            // LC example 2: the obstacle at (2, 4) stops the second walk one cell short.
            { [4, -1, 4, -2, 4], [[2, 4]], 65 },

            // LC example 3: turning in place never moves the robot.
            { [6, -1, -1, 6], [], 36 },

            // A full clockwise loop returns to the origin, so the farthest point is
            // reached mid-walk and has to be remembered.
            { [3, -1, 3, -1, 3, -1, 3], [], 18 },

            // The very first step is blocked, so the robot never leaves the origin.
            { [5], [[0, 1]], 0 },

            // Left turns wrap the direction index backwards past north.
            { [-2, 4], [], 16 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDistanceSquaredByLinearScan_LeetCodeExamples_ReturnsFarthestSquaredDistance(
        int[] commands, int[][] obstacles, int expected)
    {
        var actual = WalkingRobotSimulationSolution.MaxDistanceSquaredByLinearScan(commands, obstacles);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDistanceSquaredByObstacleSet_LeetCodeExamples_ReturnsFarthestSquaredDistance(
        int[] commands, int[][] obstacles, int expected)
    {
        var actual = WalkingRobotSimulationSolution.MaxDistanceSquaredByObstacleSet(commands, obstacles);

        Assert.Equal(expected, actual);
    }
}
