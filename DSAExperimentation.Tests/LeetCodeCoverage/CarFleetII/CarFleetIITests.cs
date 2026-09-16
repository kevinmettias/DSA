using DSAExperimentation.LeetCode.CarFleetII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CarFleetII;

// Harness only. Both the forward-scan baseline and the monotonic-stack sweep are
// CarFleetIISolution's - this file just pins them to LeetCode's published examples
// plus the equal-speed, single-car and same-time-chain cases that exercise the
// "never catches up" and "candidate already collided" branches the stack sweep pops
// on.
public sealed partial class CarFleetIITests
{
    public static TheoryData<int[][], double[]> Examples =>
        new()
        {
            { [[1, 2], [2, 1], [4, 3], [7, 2]], [1.0, -1.0, 3.0, -1.0] },
            { [[3, 4], [5, 4], [6, 3], [9, 1]], [2.0, 1.0, 1.5, -1.0] },
            { [[1, 4], [5, 4], [9, 4]], [-1.0, -1.0, -1.0] },
            { [[1, 4]], [-1.0] },
            { [[0, 5], [3, 4], [6, 3], [9, 2], [12, 1]], [3.0, 3.0, 3.0, 3.0, -1.0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetCollisionTimesByBruteForce_LeetCodeExamples_ReturnsEachCarsCollisionTime(
        int[][] cars, double[] expected) =>
        Assert.Equal(expected, CarFleetIISolution.GetCollisionTimesByBruteForce(cars));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetCollisionTimesByMonotonicStack_LeetCodeExamples_ReturnsEachCarsCollisionTime(
        int[][] cars, double[] expected) =>
        Assert.Equal(expected, CarFleetIISolution.GetCollisionTimesByMonotonicStack(cars));
}
