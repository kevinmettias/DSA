using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximizeTheDistanceBetweenPointsOnASquareBenchmarks (ARCHITECTURE 17.9): both
// arms are competing strategies for one question - the largest minimum distance between the chosen
// boundary points - so a harness whose arms disagree is timing two different problems. Setup draws
// the boundary offsets from a fixed seed and sorts them through the solution's own mapping, so the
// same point count must rebuild the same positions; neither arm mutates them.
public sealed partial class MaximizeTheDistanceBetweenPointsOnASquareBenchmarksTests
{
    private const int SmallestPointCount = 50;

    [Fact]
    public void Setup_SamePointCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_AgreesWithSortedGreedy()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.SortedGreedy());
    }

    [Fact]
    public void SortedGreedy_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortedGreedy(), harness.LinearScan());
    }

    private static MaximizeTheDistanceBetweenPointsOnASquareBenchmarks BuildHarness()
    {
        var harness = new MaximizeTheDistanceBetweenPointsOnASquareBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
