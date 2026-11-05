using DSAExperimentation.LeetCode.Solutions.CarFleetII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CarFleetII;

// Harness only. Both the forward-scan baseline and the monotonic-stack sweep are
// CarFleetIISolution's - this file just pins them to the equal-speed, single-car
// and same-time-chain cases that exercise the "never catches up" and "candidate
// already collided" branches the stack sweep pops on. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class CarFleetIISolutionTests
{
    public static TheoryData<int[][], double[]> Cases =>
        new()
        {
            { [[1, 4], [5, 4], [9, 4]], [-1.0, -1.0, -1.0] },
            { [[1, 4]], [-1.0] },
            { [[0, 5], [3, 4], [6, 3], [9, 2], [12, 1]], [3.0, 3.0, 3.0, 3.0, -1.0] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetCollisionTimesByBruteForce_Cases_ReturnsEachCarsCollisionTime(
        int[][] cars, double[] expected) =>
        Assert.Equal(expected, CarFleetIISolution.GetCollisionTimesByBruteForce(cars));

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetCollisionTimesByMonotonicStack_Cases_ReturnsEachCarsCollisionTime(
        int[][] cars, double[] expected) =>
        Assert.Equal(expected, CarFleetIISolution.GetCollisionTimesByMonotonicStack(cars));
}
