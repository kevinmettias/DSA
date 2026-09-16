using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfVisitedCellsInAGridBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the fewest cells walked from the top-left to the
// bottom-right corner of the jump grid, counting cells rather than jumps - so a harness whose arms
// disagree is timing two different problems. ReduceGraph is handed the JumpGrid [GlobalSetup] already
// prepared, so the comparison also pins that both arms search the same grid: the hoisted grid must
// carry exactly the values the scan arm reads off the raw jagged array. Setup draws those values from
// one fixed seed, so the same Side must rebuild the same grid.
public sealed partial class MinimumNumberOfVisitedCellsInAGridBenchmarksTests
{
    private const int SmallestSide = 20;

    [Fact]
    public void Setup_SameSide_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceScan(), BuildHarness().BruteForceScan());

    [Fact]
    public void BruteForceScan_SameJumpGrid_AgreesWithReduceGraph()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReduceGraph(), harness.BruteForceScan());
    }

    [Fact]
    public void ReduceGraph_SameJumpGrid_AgreesWithBruteForceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceScan(), harness.ReduceGraph());
    }

    private static MinimumNumberOfVisitedCellsInAGridBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfVisitedCellsInAGridBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
