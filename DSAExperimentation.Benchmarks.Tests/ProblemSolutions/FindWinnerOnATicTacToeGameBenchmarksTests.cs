using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindWinnerOnATicTacToeGameBenchmarks (ARCHITECTURE 17.9): the generic arms
// check only compares the arms with each other, so this pins the baseline's verdict to one worked
// out independently of every arm. Setup shuffles every cell of LC 1275's 3 x 3 board into the move
// order, and at its seed the nine moves fill the board as XXO / OOX / XXO, which completes no line,
// so the verdict is a draw.
public sealed partial class FindWinnerOnATicTacToeGameBenchmarksTests
{
    private const int SmallestSize = 3;
    private const string ExpectedFullBoardVerdict = "Draw";

    [Fact]
    public void RebuildAndRescanEveryMove_SeededFullBoard_ReturnsDraw() =>
        Assert.Equal(ExpectedFullBoardVerdict, BuildHarness().RebuildAndRescanEveryMove());

    private static FindWinnerOnATicTacToeGameBenchmarks BuildHarness()
    {
        var harness = new FindWinnerOnATicTacToeGameBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
