using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AvailableCapturesForRookBenchmarks (ARCHITECTURE 17.9): its two arms are
// AvailableCapturesForRookSolution's competing strategies for the same question - a full-board scan of every
// square against four ray walks - so a harness whose arms disagree has searched two different boards. A rook
// captures at most one pawn per direction, so the count is bounded by four and that bound is asserted as well
// rather than resting on the arms agreeing with each other. Setup places the rook and the pawns on its row and
// column from one fixed seed, so the same Size must rebuild the same board.
public sealed partial class AvailableCapturesForRookBenchmarksTests
{
    // The smaller of Setup's [Params(50, 500)] board sizes.
    private const int SmallestSize = 50;

    // Four directions, at most one pawn captured in each before the rook is blocked.
    private const int MaxRookCaptures = 4;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameBoard() =>
        Assert.Equal(BuildHarness().FullBoardScan(), BuildHarness().FullBoardScan());

    [Fact]
    public void FullBoardScan_FiftySquareBoard_AgreesWithDirectRayWalk()
    {
        var harness = BuildHarness();

        Assert.InRange(harness.FullBoardScan(), 0, MaxRookCaptures);
        Assert.Equal(harness.DirectRayWalk(), harness.FullBoardScan());
    }

    [Fact]
    public void DirectRayWalk_FiftySquareBoard_AgreesWithFullBoardScan()
    {
        var harness = BuildHarness();

        Assert.InRange(harness.DirectRayWalk(), 0, MaxRookCaptures);
        Assert.Equal(harness.FullBoardScan(), harness.DirectRayWalk());
    }

    private static AvailableCapturesForRookBenchmarks BuildHarness()
    {
        var harness = new AvailableCapturesForRookBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
