using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ZeroOneMatrixBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for one question - every cell's distance to its nearest zero - so a harness whose arms
// disagree is timing two different problems. Neither arm writes into the input matrix, so one
// harness instance is safe to call twice in either order. Setup pins one cell to zero, which is the
// decisive value: a plotted distance at a zero cell is zero.
public sealed partial class ZeroOneMatrixBenchmarksTests
{
    private const int SmallestSize = 10;
    private const int PinnedZeroRow = 0;
    private const int PinnedZeroColumn = 0;
    private const int ZeroCellDistance = 0;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().PerCellBfs()),
            AnswerText.Of(BuildHarness().PerCellBfs()));

    [Fact]
    public void PerCellBfs_AgreesWithMultiSourceBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.PerCellBfs()),
            AnswerText.Of(harness.MultiSourceBfs()));
    }

    [Fact]
    public void MultiSourceBfs_AgreesWithPerCellBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MultiSourceBfs()),
            AnswerText.Of(harness.PerCellBfs()));
    }

    [Fact]
    public void PerCellBfs_PinnedZeroCell_ReportsZeroDistance() =>
        Assert.Equal(ZeroCellDistance, BuildHarness().PerCellBfs()[PinnedZeroRow][PinnedZeroColumn]);

    [Fact]
    public void MultiSourceBfs_PinnedZeroCell_ReportsZeroDistance() =>
        Assert.Equal(ZeroCellDistance, BuildHarness().MultiSourceBfs()[PinnedZeroRow][PinnedZeroColumn]);

    private static ZeroOneMatrixBenchmarks BuildHarness()
    {
        var harness = new ZeroOneMatrixBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
