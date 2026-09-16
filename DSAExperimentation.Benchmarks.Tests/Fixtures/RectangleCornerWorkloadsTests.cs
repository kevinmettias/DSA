using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for RectangleCornerWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3235's
// circles being scattered across the fixed scenario rectangle with a radius large enough relative to the
// coordinate spread that a meaningful fraction actually overlap: a radius tiny compared to the spread
// would make every circle its own isolated component and let both strategies skip the overlap work.
public sealed partial class RectangleCornerWorkloadsTests
{
    private const int CircleCount = 50;
    private const int Seed = 3235;
    private const int CircleFieldCount = 3; // x, y, r
    private const int RadiusFieldIndex = 2;
    private const int MinRadius = 20;
    private const int MaxRadius = 80;
    private const int FirstCoordinate = 1;

    // The overlap claim is about how often two circles meet, so it needs a larger draw than the
    // benchmark's own count: at fifty circles a pair is still uncommon enough that a fresh seed could
    // plausibly leave none, which would make the assertion a coin flip rather than a property.
    private const int OverlapSampleCount = 200;

    [Fact]
    public void BuildCircles_CircleCount_ReturnsOneThreeFieldCirclePerPosition()
    {
        var circles = RectangleCornerWorkloads.BuildCircles(CircleCount, Seed);

        Assert.Equal(CircleCount, circles.Length);
        Assert.All(circles, circle => Assert.Equal(CircleFieldCount, circle.Length));
    }

    [Fact]
    public void BuildCircles_EveryCircle_LiesInsideTheScenarioRectangleWithARadiusInsideTheDocumentedBand()
    {
        var circles = RectangleCornerWorkloads.BuildCircles(CircleCount, Seed);

        Assert.All(circles, circle => Assert.InRange(circle[0], FirstCoordinate, RectangleCornerScenario.XCorner - 1));
        Assert.All(circles, circle => Assert.InRange(circle[1], FirstCoordinate, RectangleCornerScenario.YCorner - 1));
        Assert.All(circles, circle => Assert.InRange(circle[RadiusFieldIndex], MinRadius, MaxRadius));
    }

    [Fact]
    public void BuildCircles_LargerSample_LeavesCirclesOverlappingSoComponentsAreReal() =>
        Assert.True(HasOverlappingPair(RectangleCornerWorkloads.BuildCircles(OverlapSampleCount, Seed)));

    [Fact]
    public void BuildCircles_SameSeed_ReturnsTheSameCircles() =>
        Assert.Equal(
            AnswerText.Of(RectangleCornerWorkloads.BuildCircles(CircleCount, Seed)),
            AnswerText.Of(RectangleCornerWorkloads.BuildCircles(CircleCount, Seed)));

    private static bool HasOverlappingPair(int[][] circles)
    {
        for (var first = 0; first < circles.Length; first++)
        {
            for (var second = first + 1; second < circles.Length; second++)
            {
                if (Overlap(circles[first], circles[second]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool Overlap(int[] first, int[] second)
    {
        var dx = first[0] - second[0];
        var dy = first[1] - second[1];
        var radii = first[RadiusFieldIndex] + second[RadiusFieldIndex];

        return (dx * dx) + (dy * dy) < (radii * radii);
    }
}
