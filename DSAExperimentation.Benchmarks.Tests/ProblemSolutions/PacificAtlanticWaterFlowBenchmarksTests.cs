using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PacificAtlanticWaterFlowBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same cell list - a per-cell downhill DFS against one reverse-flow
// flood fill per ocean - so a harness whose arms disagree is timing two different problems. Setup
// draws the grid from one seeded Random, so the same Size must rebuild the same heights. Both arms
// collect their cells row-major, so the problem's cell order is the order both answers carry and the
// default order-sensitive rendering is the right comparison; the smallest tuned Size is used.
public sealed partial class PacificAtlanticWaterFlowBenchmarksTests
{
    private const int SmallestSize = 10;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().MultiSourceFloodFill()),
            AnswerText.Of(BuildHarness().MultiSourceFloodFill()));

    [Fact]
    public void PerCellDfs_SmallestSize_AgreesWithMultiSourceFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MultiSourceFloodFill()),
            AnswerText.Of(harness.PerCellDfs()));
    }

    [Fact]
    public void MultiSourceFloodFill_SmallestSize_AgreesWithPerCellDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.PerCellDfs()),
            AnswerText.Of(harness.MultiSourceFloodFill()));
    }

    private static PacificAtlanticWaterFlowBenchmarks BuildHarness()
    {
        var harness = new PacificAtlanticWaterFlowBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
