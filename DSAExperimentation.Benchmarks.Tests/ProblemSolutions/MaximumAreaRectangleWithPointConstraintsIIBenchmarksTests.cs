using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumAreaRectangleWithPointConstraintsIIBenchmarks (ARCHITECTURE 17.9):
// both arms are competing strategies for one question - the largest axis-aligned rectangle whose
// four corners are all present, with no other given point inside or on its border - so a harness
// whose arms disagree is timing two different problems. Setup builds the dense grid and the sorted
// Point[] the sweep arm is handed, so the same grid side must rebuild the same workload; neither arm
// mutates either structure.
public sealed partial class MaximumAreaRectangleWithPointConstraintsIIBenchmarksTests
{
    private const int SmallestGridSide = 4;

    [Fact]
    public void Setup_SameGridSide_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().QuadrupleScan(), BuildHarness().QuadrupleScan());

    [Fact]
    public void QuadrupleScan_AgreesWithSweepSegmentTree()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.QuadrupleScan(), harness.SweepSegmentTree());
    }

    [Fact]
    public void SweepSegmentTree_AgreesWithQuadrupleScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SweepSegmentTree(), harness.QuadrupleScan());
    }

    private static MaximumAreaRectangleWithPointConstraintsIIBenchmarks BuildHarness()
    {
        var harness = new MaximumAreaRectangleWithPointConstraintsIIBenchmarks { GridSide = SmallestGridSide };
        harness.Setup();

        return harness;
    }
}
