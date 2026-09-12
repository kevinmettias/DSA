using static DSAExperimentation.LeetCode.GenerateRandomPointInACircle.GenerateRandomPointInACircleSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GenerateRandomPointInACircle;

// Harness only: both strategies live in GenerateRandomPointInACircleSolution.
// RandPoint()'s result is nondeterministic, so each example asserts the
// invariants a valid draw must satisfy (lands inside the circle; degenerates to
// the exact center when radius is zero; produces enough distinct points to rule
// out a constant or narrow draw) rather than one exact value, the same
// "expected slot carries a candidate set" idea RandomPickIndexTests already
// uses for Pick.
public sealed class GenerateRandomPointInACircleTests
{
    private const double ContainmentTolerance = 1e-9;

    public static TheoryData<double, double, double, int, int> Examples =>
        new()
        {
            // radius, xCenter, yCenter, trials, minimumDistinctPointsExpected
            { 10.0, 5.0, -3.0, 500, 1 },
            { 1.0, 0.0, 0.0, 200, 190 },
            { 0.0, 4.0, 7.0, 50, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RandPoint_LeetCodeExamplesByRejectionSampling_AlwaysLandsWithinTheCircleAndVaries(
        double radius, double xCenter, double yCenter, int trials, int minimumDistinctPoints)
        => AssertRandPointBehavesCorrectly(
            new GenerateRandomPointInACircleByRejectionSampling(radius, xCenter, yCenter),
            radius, xCenter, yCenter, trials, minimumDistinctPoints);

    [Theory]
    [MemberData(nameof(Examples))]
    public void RandPoint_LeetCodeExamplesByClosedFormPolar_AlwaysLandsWithinTheCircleAndVaries(
        double radius, double xCenter, double yCenter, int trials, int minimumDistinctPoints)
        => AssertRandPointBehavesCorrectly(
            new GenerateRandomPointInACircleByClosedFormPolar(radius, xCenter, yCenter),
            radius, xCenter, yCenter, trials, minimumDistinctPoints);

    private static void AssertRandPointBehavesCorrectly(
        IRandomPointGenerator generator,
        double radius,
        double xCenter,
        double yCenter,
        int trials,
        int minimumDistinctPoints)
    {
        var distinct = new HashSet<(double X, double Y)>();

        for (var i = 0; i < trials; i++)
        {
            var point = generator.RandPoint();
            var dx = point[0] - xCenter;
            var dy = point[1] - yCenter;

            Assert.True((dx * dx) + (dy * dy) <= (radius * radius) + ContainmentTolerance);

            if (radius == 0.0)
            {
                Assert.Equal(xCenter, point[0]);
                Assert.Equal(yCenter, point[1]);
            }

            distinct.Add((point[0], point[1]));
        }

        Assert.True(distinct.Count >= minimumDistinctPoints);
    }
}
