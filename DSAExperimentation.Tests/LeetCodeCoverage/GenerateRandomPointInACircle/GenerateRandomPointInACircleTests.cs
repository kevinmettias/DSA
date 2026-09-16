using DSAExperimentation.LeetCode.GenerateRandomPointInACircle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GenerateRandomPointInACircle;

// Harness only: both strategies live in GenerateRandomPointInACircleSolution.
// RandPoint()'s result is nondeterministic, so each example asserts the
// invariants a valid draw must satisfy (lands inside the circle; degenerates to
// the exact center when radius is zero; produces enough distinct points to rule
// out a constant or narrow draw) rather than one exact value, the same
// "expected slot carries a candidate set" idea RandomPickIndexTests already
// uses for Pick.
//
// The two strategies are reached through the solution class rather than a
// `using static`: a wildcard import drops every member in as a bare identifier,
// so a reader meeting GenerateRandomPointInACircleByClosedFormPolar has nothing
// on the line telling them whose it is.
public sealed partial class GenerateRandomPointInACircleTests
{
    private const double ContainmentTolerance = 1e-9;

    public static TheoryData<RandPointCase> Examples =>
        new()
        {
            { new RandPointCase(Radius: 10.0, XCenter: 5.0, YCenter: -3.0, Trials: 500, MinimumDistinctPoints: 1) },
            { new RandPointCase(Radius: 1.0, XCenter: 0.0, YCenter: 0.0, Trials: 200, MinimumDistinctPoints: 190) },
            { new RandPointCase(Radius: 0.0, XCenter: 4.0, YCenter: 7.0, Trials: 50, MinimumDistinctPoints: 1) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RandPoint_LeetCodeExamplesByRejectionSampling_AlwaysLandsWithinTheCircleAndVaries(
        RandPointCase example) =>
        AssertRandPointBehavesCorrectly(
            new GenerateRandomPointInACircleSolution.GenerateRandomPointInACircleByRejectionSampling(
                example.Radius, example.XCenter, example.YCenter),
            example);

    [Theory]
    [MemberData(nameof(Examples))]
    public void RandPoint_LeetCodeExamplesByClosedFormPolar_AlwaysLandsWithinTheCircleAndVaries(
        RandPointCase example) =>
        AssertRandPointBehavesCorrectly(
            new GenerateRandomPointInACircleSolution.GenerateRandomPointInACircleByClosedFormPolar(
                example.Radius, example.XCenter, example.YCenter),
            example);

    private static void AssertRandPointBehavesCorrectly(
        GenerateRandomPointInACircleSolution.IRandomPointGenerator generator,
        RandPointCase example)
    {
        var distinct = new HashSet<(double X, double Y)>();

        for (var i = 0; i < example.Trials; i++)
        {
            var point = generator.RandPoint();
            var dx = point[0] - example.XCenter;
            var dy = point[1] - example.YCenter;

            Assert.True((dx * dx) + (dy * dy) <= (example.Radius * example.Radius) + ContainmentTolerance);

            if (example.Radius == 0.0)
            {
                Assert.Equal(example.XCenter, point[0]);
                Assert.Equal(example.YCenter, point[1]);
            }

            distinct.Add((point[0], point[1]));
        }

        Assert.True(distinct.Count >= example.MinimumDistinctPoints);
    }

    // One LeetCode example: the circle to draw from, how many draws to replay, and the
    // fewest distinct points those draws must produce between them. Every value is named
    // where it is passed, so three adjacent doubles are read as radius, x and y rather
    // than as three interchangeable positions. Nested because it is only ever used inside
    // this test class - it is this harness's own vocabulary, not a type another file
    // would import.
    public readonly record struct RandPointCase(
        double Radius, double XCenter, double YCenter, int Trials, int MinimumDistinctPoints);
}
