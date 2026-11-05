using DSAExperimentation.LeetCode.Solutions.RobotCollisions;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RobotCollisions;

// Harness only: both strategies are RobotCollisionsSolution's, the same
// restart-the-scan baseline and single-sweep stack simulation
// RobotCollisionsBenchmarks measures.
public sealed partial class RobotCollisionsSolutionTests
{
    public static TheoryData<int[], int[], string, int[]> Cases =>
        new()
        {
            // One left-mover cascading through three right-movers, losing one health per kill.
            { [1, 2, 3, 4], [5, 3, 1, 10], "RRRL", [7] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SurvivorHealthsByRepeatedScan_Cases_ReturnsSurvivingHealthsInInputOrder(
        int[] positions, int[] healths, string directions, int[] expected)
    {
        var actual = RobotCollisionsSolution.SurvivorHealthsByRepeatedScan(positions, healths, directions);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SurvivorHealthsByStackSimulation_Cases_ReturnsSurvivingHealthsInInputOrder(
        int[] positions, int[] healths, string directions, int[] expected)
    {
        var actual = RobotCollisionsSolution.SurvivorHealthsByStackSimulation(positions, healths, directions);

        Assert.Equal(expected, actual);
    }
}
