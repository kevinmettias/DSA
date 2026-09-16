using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RandomFlipMatrixBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - uniformly picking every cell of a 1 x Cells matrix without
// replacement - so a harness whose arms disagree is timing two different problems.
//
// The agreement is weaker than the class comment claims, and this is said plainly rather than
// dressed up. Each arm builds its own matrix inside the call with its own Random(1), so one harness
// is safe to call twice in either order; but both drains run exactly Cells flips over a matrix of
// exactly Cells cells, so each arm picks every value 0..Cells-1 exactly once and the checksum is
// fixed at Cells*(Cells-1)/2 no matter what order either arm picked in. The checksum therefore
// witnesses that both arms covered the whole cell range exactly once - a strategy that repeated a
// cell or stopped early fails it - not that they picked the same sequence. Reporting a stronger
// claim would need the picked sequence itself, which the harness's long return type discards.
//
// There is no [GlobalSetup] here: a Design problem's workload is the Cells parameter alone.
public sealed partial class RandomFlipMatrixBenchmarksTests
{
    private const int SmallestCells = 200;

    [Fact]
    public void ListScan_SmallestCellCount_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListScan(), harness.HashMapSwapRemove());
    }

    [Fact]
    public void HashMapSwapRemove_SmallestCellCount_AgreesWithTheListScanArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapSwapRemove(), harness.ListScan());
    }

    private static RandomFlipMatrixBenchmarks BuildHarness() =>
        new() { Cells = SmallestCells };
}
