using DSAExperimentation.Domain.Games.TicTacToe;
using DSAExperimentation.LeetCode.FindWinnerOnATicTacToeGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindWinnerOnATicTacToeGameSolution's, the same
// methods FindWinnerOnATicTacToeGameSolutionTests proves correct. LC 1275 fixes the
// board at 3 x 3 and so at nine moves, which leaves one size to run, and Size is
// that board's - Domain/Games/TicTacToe's BoardSize, the board both strategies play on.
// The workload shuffles every cell of the board into a move
// order in [GlobalSetup], so only the replay is measured; at this seed the shuffle
// completes no line - LC 1275 allows no move after a win - so both arms walk all
// nine moves and report a draw.
public class FindWinnerOnATicTacToeGameBenchmarks
{
    // LC problem number, reused as the deterministic move-order seed.
    private const int MoveSeed = 1275;

    private int[][] _moves = [];

    [Params(TicTacToeRules.BoardSize)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var cells = new List<int[]>(Size * Size);

        for (var row = 0; row < Size; row++)
        {
            for (var column = 0; column < Size; column++)
            {
                cells.Add([row, column]);
            }
        }

        var random = new Random(MoveSeed);

        for (var i = cells.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (cells[i], cells[j]) = (cells[j], cells[i]);
        }

        _moves = [.. cells];
    }

    [Benchmark(Baseline = true)]
    public string RebuildAndRescanEveryMove() =>
        FindWinnerOnATicTacToeGameSolution.FindWinnerByBoardRescan(_moves);

    [Benchmark]
    public string IncrementalRunningCounts() =>
        FindWinnerOnATicTacToeGameSolution.FindWinnerByRunningCounts(_moves);
}
