using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ZeroOneMatrixBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms
// to agreeing with each other, which a shared wrong distance map would pass. Setup pins one cell to
// zero, which is the decisive value pinned on the baseline: a plotted distance at a zero cell is
// zero.
public sealed partial class ZeroOneMatrixBenchmarksTests
{
    private const int SmallestSize = 10;
    private const int PinnedZeroRow = 0;
    private const int PinnedZeroColumn = 0;
    private const int ZeroCellDistance = 0;

    [Fact]
    public void PerCellBfs_PinnedZeroCell_ReportsZeroDistance() =>
        Assert.Equal(ZeroCellDistance, BuildHarness().PerCellBfs()[PinnedZeroRow][PinnedZeroColumn]);

    private static ZeroOneMatrixBenchmarks BuildHarness()
    {
        var harness = new ZeroOneMatrixBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
