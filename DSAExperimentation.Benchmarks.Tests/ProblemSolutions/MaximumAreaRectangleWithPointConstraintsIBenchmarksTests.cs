using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumAreaRectangleWithPointConstraintsIBenchmarks (ARCHITECTURE 17.9): both
// arms are competing strategies for one question - the largest axis-aligned rectangle whose four
// corners are all present, with nothing else inside the box - so a harness whose arms disagree is
// timing two different problems. Setup builds the dense grid and the prepared corner set the
// lookup arm is handed, so the same grid side must rebuild the same workload; neither arm mutates
// either structure.
public sealed partial class MaximumAreaRectangleWithPointConstraintsIBenchmarksTests
{
    private const int SmallestGridSide = 4;

    [Fact]
    public void Setup_SameGridSide_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().QuadrupleScan(), BuildHarness().QuadrupleScan());

    [Fact]
    public void QuadrupleScan_AgreesWithCornerLookup()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.QuadrupleScan(), harness.CornerLookup());
    }

    [Fact]
    public void CornerLookup_AgreesWithQuadrupleScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CornerLookup(), harness.QuadrupleScan());
    }

    private static MaximumAreaRectangleWithPointConstraintsIBenchmarks BuildHarness()
    {
        var harness = new MaximumAreaRectangleWithPointConstraintsIBenchmarks { GridSide = SmallestGridSide };
        harness.Setup();

        return harness;
    }
}
