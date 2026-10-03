using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindWinnerOnATicTacToeGameBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. The workload is the same seeded shuffle of every cell, so the same Size must
// rebuild the same move order.
//
// The agreement here is weak by construction and the assertions say only what the fixture supports:
// Setup shuffles every cell of LC 1275's 3 x 3 board into the move order, and at its seed the nine
// moves - worked through independently of either arm - fill the board as XXO / OOX / XXO, which
// completes no line, so neither arm has a completed line to report. What the two calls agree on is
// therefore the full-board verdict, not a mid-game winner - an arm that stopped scanning early
// would still be caught, but an arm that mis-detects a *row* verdict would not be.
public sealed partial class FindWinnerOnATicTacToeGameBenchmarksTests
{
    private const int SmallestSize = 3;
    private const string ExpectedFullBoardVerdict = "Draw";

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().RebuildAndRescanEveryMove(),
            BuildHarness().RebuildAndRescanEveryMove());

    [Fact]
    public void RebuildAndRescanEveryMove_AgreesWithIncrementalRunningCounts()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedFullBoardVerdict, harness.RebuildAndRescanEveryMove());
        Assert.Equal(harness.IncrementalRunningCounts(), harness.RebuildAndRescanEveryMove());
    }

    private static FindWinnerOnATicTacToeGameBenchmarks BuildHarness()
    {
        var harness = new FindWinnerOnATicTacToeGameBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
