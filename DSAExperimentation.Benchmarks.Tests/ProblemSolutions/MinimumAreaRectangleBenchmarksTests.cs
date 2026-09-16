using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumAreaRectangleBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a linear scan of every earlier point for each
// candidate corner against a hash set of XY keys - so a harness whose arms disagree is timing two
// different problems. Setup draws the points from a grid barely larger than the point count, so
// rectangles are plentiful and a degenerate all-collinear workload cannot make both arms answer the
// same sentinel for the wrong reason. The point set is seeded, so the same Length must rebuild it.
public sealed partial class MinimumAreaRectangleBenchmarksTests
{
    private const int SmallestLength = 60;

    [Fact]
    public void Setup_SameLength_RebuildsTheSamePointSet() =>
        Assert.Equal(BuildHarness().LinearScanLookup(), BuildHarness().LinearScanLookup());

    [Fact]
    public void LinearScanLookup_DenseGridPoints_AgreesWithSetLookup()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SetLookup(), harness.LinearScanLookup());
    }

    [Fact]
    public void SetLookup_DenseGridPoints_AgreesWithLinearScanLookup()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanLookup(), harness.SetLookup());
    }

    private static MinimumAreaRectangleBenchmarks BuildHarness()
    {
        var harness = new MinimumAreaRectangleBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
