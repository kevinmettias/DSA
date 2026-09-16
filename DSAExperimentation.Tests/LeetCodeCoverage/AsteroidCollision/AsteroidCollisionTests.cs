using DSAExperimentation.LeetCode.AsteroidCollision;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AsteroidCollision;

// Harness only: both strategies live in AsteroidCollisionSolution and are
// asserted against the same examples, including the cascading-collision case.
public sealed partial class AsteroidCollisionTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [5, 10, -5], [5, 10] },
            { [8, -8], [] },
            { [10, 2, -5], [10] },
            { [-2, -1, 1, 2], [-2, -1, 1, 2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SimulateByRepeatedScan_LeetCodeExamples_ReturnsSurvivors(int[] asteroids, int[] expected) =>
        Assert.Equal(expected, AsteroidCollisionSolution.SimulateByRepeatedScan(asteroids));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SimulateByStackPass_LeetCodeExamples_ReturnsSurvivors(int[] asteroids, int[] expected) =>
        Assert.Equal(expected, AsteroidCollisionSolution.SimulateByStackPass(asteroids));
}
