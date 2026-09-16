using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountLatticePointsInsideACircleBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - one union of every circle's points - so a
// harness whose arms disagree is timing two different problems. Setup seeds the circles, so the
// same CircleCount must rebuild the same ones.
public sealed partial class CountLatticePointsInsideACircleBenchmarksTests
{
    private const int SmallestCircleCount = 50;

    [Fact]
    public void Setup_SmallestCircleCount_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: every circle's radius is drawn from [1, 6), so each one covers at
        // least its own centre and the union the answer counts can never be empty.
        Assert.InRange(first.FullGridScan(), 1, int.MaxValue);
        Assert.Equal(first.FullGridScan(), second.FullGridScan());
    }

    [Fact]
    public void FullGridScan_ScatteredCircles_AgreesWithPerCircleBoundingBoxWithSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PerCircleBoundingBoxWithSet(), harness.FullGridScan());
    }

    [Fact]
    public void PerCircleBoundingBoxWithSet_ScatteredCircles_AgreesWithFullGridScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FullGridScan(), harness.PerCircleBoundingBoxWithSet());
    }

    private static CountLatticePointsInsideACircleBenchmarks BuildHarness()
    {
        var harness = new CountLatticePointsInsideACircleBenchmarks { CircleCount = SmallestCircleCount };
        harness.Setup();

        return harness;
    }
}
