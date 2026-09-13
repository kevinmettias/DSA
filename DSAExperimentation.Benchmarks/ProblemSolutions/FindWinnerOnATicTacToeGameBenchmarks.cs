using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindWinnerOnATicTacToeGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindWinnerOnATicTacToeGameSolution's, the same
// methods FindWinnerOnATicTacToeGameTests proves correct, called through the
// board-size overload so BenchmarkDotNet has something to scale via [Params] -
// LC 1275 itself fixes n = 3. The workload shuffles every cell of an n x n board
// into a move order in [GlobalSetup], so only the replay is measured; a completed
// line is vanishingly unlikely under a random alternating fill, so both arms walk
// every move and report a draw.
[MemoryDiagnoser]
public class FindWinnerOnATicTacToeGameBenchmarks
{
    // LC problem number, reused as the deterministic move-order seed.
    private const int MoveSeed = 1275;

    [Params(10, 60)]
    public int Size;

    private int[][] _moves = null!;

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
        FindWinnerOnATicTacToeGameSolution.FindWinnerByBoardRescan(_moves, Size);

    [Benchmark]
    public string IncrementalRunningCounts() =>
        FindWinnerOnATicTacToeGameSolution.FindWinnerByRunningCounts(_moves, Size);
}
