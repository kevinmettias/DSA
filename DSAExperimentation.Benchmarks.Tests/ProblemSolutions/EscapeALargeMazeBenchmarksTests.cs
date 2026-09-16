using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for EscapeALargeMazeBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same board - a materialized flood fill against the capped implicit-graph
// traversal - so a harness whose arms disagree is answering two different mazes. Both arms return
// bool, and Setup scatters its blocked cells strictly inside the board so neither corner is ever
// sealed in, which is what makes the documented answer true rather than a tautology. Setup's
// blocked cells are drawn from a fixed seed, so the same BoardSize must rebuild the same workload.
public sealed partial class EscapeALargeMazeBenchmarksTests
{
    private const int SmallestBoardSize = 500;

    // Both arms' documented outcome: blocked cells are kept away from both corners, so the walk
    // from one corner to the other is always open.
    private const bool ExpectedCanEscape = true;

    [Fact]
    public void Setup_SameBoardSize_RebuildsTheSameBlockedCells()
    {
        Assert.Equal(ExpectedCanEscape, BuildHarness().CanEscapeByCappedTraversal());

        Assert.Equal(BuildHarness().CanEscapeByCappedTraversal(), BuildHarness().CanEscapeByCappedTraversal());
    }

    [Fact]
    public void CanEscapeByFullBoardFloodFill_OpenCornerBoard_AgreesWithCappedTraversal()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanEscapeByCappedTraversal(), harness.CanEscapeByFullBoardFloodFill());
    }

    [Fact]
    public void CanEscapeByCappedTraversal_OpenCornerBoard_AgreesWithFullBoardFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanEscapeByFullBoardFloodFill(), harness.CanEscapeByCappedTraversal());
    }

    private static EscapeALargeMazeBenchmarks BuildHarness()
    {
        var harness = new EscapeALargeMazeBenchmarks { BoardSize = SmallestBoardSize };
        harness.Setup();

        return harness;
    }
}
