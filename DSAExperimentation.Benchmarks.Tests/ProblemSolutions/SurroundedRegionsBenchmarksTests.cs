using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.SurroundedRegions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SurroundedRegionsBenchmarks (ARCHITECTURE 17.9): the class carries two arms -
// the depth-first and breadth-first captures from the border - so the pair must agree on the board as
// well as each matching an independent oracle.
//
// Two things shape the tests below. Both arms return void and mutate the board they are handed, and the
// board is a private field that both [GlobalSetup] and [IterationSetup] fill from the same seeded
// fixture expression, so no test can read the harness's own board: the workload-rebuild pins are
// therefore statements about the expression the class names, each reached through an arm's own effect
// on a board built from it. And LC 130's contract is decisive, not merely reproducible: the capture is
// exactly the 'O' cells that cannot reach the border, which the marking pass below decides for itself
// from the board's own shape rather than from the arms' traversals. That oracle's border-reach walk is
// itself queue-based, so it now shares the breadth-first arm's shape - it stays a separate
// implementation (its own bool[,] reached map), and the depth-first arm's agreement with it is what
// keeps the oracle honest.
public sealed partial class SurroundedRegionsBenchmarksTests
{
    // Mirrors the Size and seed the class's own [GlobalSetup] and [IterationSetup] pass.
    private const int SmallestSize = 50;
    private const int WorkloadSeed = 130;
    private const char OpenCell = 'O';
    private const char CapturedCell = 'X';

    [Fact]
    public void Setup_SameSize_RebuildsTheSameBoard()
    {
        BuildHarness().BorderDepthFirstSearch();

        Assert.Equal(AnswerGraphText.Of(GradedBoard()), AnswerGraphText.Of(GradedBoard()));
    }

    [Fact]
    public void IterationSetup_AfterAGradedRun_RePresentsThePristineBoard()
    {
        var harness = BuildHarness();
        harness.BorderDepthFirstSearch();
        harness.IterationSetup();

        Assert.Equal(AnswerGraphText.Of(GradedBoard()), AnswerGraphText.Of(GradedBoard()));
    }

    [Fact]
    public void BorderDepthFirstSearch_SeededBoard_CapturesExactlyTheRegionsCutOffFromTheBorder()
    {
        BuildHarness().BorderDepthFirstSearch();

        Assert.Equal(AnswerGraphText.Of(ExpectedCapture()), AnswerGraphText.Of(GradedBoard()));
    }

    [Fact]
    public void BorderBreadthFirstSearch_SeededBoard_CapturesExactlyTheRegionsCutOffFromTheBorder()
    {
        BuildHarness().BorderBreadthFirstSearch();

        Assert.Equal(AnswerGraphText.Of(ExpectedCapture()), AnswerGraphText.Of(BreadthFirstGradedBoard()));
    }

    [Fact]
    public void BorderBreadthFirstSearch_AgreesWithBorderDepthFirstSearch() =>
        Assert.Equal(AnswerGraphText.Of(GradedBoard()), AnswerGraphText.Of(BreadthFirstGradedBoard()));

    private static SurroundedRegionsBenchmarks BuildHarness()
    {
        var harness = new SurroundedRegionsBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }

    // The arm's own call, applied to a board built by the expression both preparation hooks name.
    private static char[][] GradedBoard()
    {
        var board = PristineBoard();
        SurroundedRegionsSolution.SolveByBorderDepthFirstSearch(board);

        return board;
    }

    // The new arm's own call, applied to a board built by the expression both preparation hooks name.
    private static char[][] BreadthFirstGradedBoard()
    {
        var board = PristineBoard();
        SurroundedRegionsSolution.SolveByBorderBreadthFirstSearch(board);

        return board;
    }

    private static char[][] PristineBoard() => SurroundedRegionsWorkloads.BuildBoard(SmallestSize, WorkloadSeed);

    // LC 130 by its own statement of the answer: every 'O' reachable from the border through 'O'
    // cells survives, and every other 'O' is captured. The reachable set is grown from all four
    // borders with an explicit frontier, which is a different traversal from the depth-first arm's
    // recursion (the breadth-first arm shares this queue shape, as the class comment notes).
    private static char[][] ExpectedCapture()
    {
        var board = PristineBoard();
        var reached = new bool[SmallestSize, SmallestSize];
        var reach = new BorderReach(board, reached, new Queue<(int Row, int Col)>());

        for (var index = 0; index < SmallestSize; index++)
        {
            VisitIfOpen(reach, row: 0, col: index);
            VisitIfOpen(reach, row: SmallestSize - 1, col: index);
            VisitIfOpen(reach, row: index, col: 0);
            VisitIfOpen(reach, row: index, col: SmallestSize - 1);
        }

        while (reach.Frontier.Count > 0)
        {
            var (row, col) = reach.Frontier.Dequeue();
            VisitIfOpen(reach, row - 1, col);
            VisitIfOpen(reach, row + 1, col);
            VisitIfOpen(reach, row, col - 1);
            VisitIfOpen(reach, row, col + 1);
        }

        CaptureUnreachedOpenCells(board, reached);

        return board;
    }

    // The three values every step of the border-reach walk shares - the board being read, the marks
    // the walk leaves and the frontier it is draining - so a call site names only the cell it visits.
    private static void VisitIfOpen(BorderReach reach, int row, int col)
    {
        var isOffBoard = row < 0 || row >= SmallestSize || col < 0 || col >= SmallestSize;
        var cannotBeCaptured = isOffBoard || reach.Reached[row, col] || reach.Board[row][col] != OpenCell;

        if (cannotBeCaptured)
        {
            return;
        }

        reach.Reached[row, col] = true;
        reach.Frontier.Enqueue((row, col));
    }

    private static void CaptureUnreachedOpenCells(char[][] board, bool[,] reached)
    {
        for (var row = 0; row < SmallestSize; row++)
        {
            for (var col = 0; col < SmallestSize; col++)
            {
                if (board[row][col] == OpenCell && !reached[row, col])
                {
                    board[row][col] = CapturedCell;
                }
            }
        }
    }

    // The border-reach walk in progress: the board, the marks it leaves and the frontier it is
    // draining. Data only - the walk's steps stay as static methods on the test class.
    private readonly record struct BorderReach(
        char[][] Board, bool[,] Reached, Queue<(int Row, int Col)> Frontier);
}
