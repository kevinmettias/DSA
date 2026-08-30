using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Winner on a Tic Tac Toe Game (LC 1275): rebuilding the board and rescanning
// every row/column/diagonal from scratch after each move vs. the standard
// incremental row/column/diagonal running-count check FindWinnerOnATicTacToeGameTests
// uses (touch only the one row, one column, and up to two diagonals the new move
// actually falls on). Generalized to an n x n board purely to give BenchmarkDotNet
// something to scale via [Params] - LC1275 itself fixes n=3. No repo primitive
// applies to either shape (Grid/GridNode model pathfinding adjacency for graph
// walks, not score tracking), the same "two genuinely different algorithms, no
// data structure" precedent SpiralMatrixBenchmarks/RotateImageBenchmarks already
// set for this folder.
[MemoryDiagnoser]
public class FindWinnerOnATicTacToeGameBenchmarks
{
    [Params(10, 60)]
    public int Size;

    private (int Row, int Col)[] _moves = null!;

    [GlobalSetup]
    public void Setup()
    {
        var cells = new List<(int Row, int Col)>(Size * Size);

        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                cells.Add((r, c));
            }
        }

        var random = new Random(1);
        for (var i = cells.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (cells[i], cells[j]) = (cells[j], cells[i]);
        }

        _moves = [.. cells];
    }

    [Benchmark(Baseline = true)]
    public string RebuildAndRescanEveryMove()
    {
        var board = new int[Size, Size];
        var lastResult = "Pending";

        for (var i = 0; i < _moves.Length; i++)
        {
            var (row, col) = _moves[i];
            board[row, col] = i % 2 == 0 ? 1 : -1;

            var winner = ScanAllLines(board, Size);
            if (winner != 0)
            {
                lastResult = winner == 1 ? "A" : "B";
            }
        }

        return lastResult;
    }

    private static int ScanAllLines(int[,] board, int size)
    {
        for (var r = 0; r < size; r++)
        {
            var winner = LineWinner(board, size, r, 0, 0, 1);
            if (winner != 0)
            {
                return winner;
            }
        }

        for (var c = 0; c < size; c++)
        {
            var winner = LineWinner(board, size, 0, c, 1, 0);
            if (winner != 0)
            {
                return winner;
            }
        }

        var diagWinner = LineWinner(board, size, 0, 0, 1, 1);
        if (diagWinner != 0)
        {
            return diagWinner;
        }

        return LineWinner(board, size, 0, size - 1, 1, -1);
    }

    private static int LineWinner(int[,] board, int size, int startRow, int startCol, int dRow, int dCol)
    {
        var first = board[startRow, startCol];

        if (first == 0)
        {
            return 0;
        }

        for (var i = 1; i < size; i++)
        {
            if (board[startRow + (i * dRow), startCol + (i * dCol)] != first)
            {
                return 0;
            }
        }

        return first;
    }

    [Benchmark]
    public string IncrementalRunningCounts()
    {
        var rowCount = new int[Size];
        var colCount = new int[Size];
        var diag = 0;
        var antiDiag = 0;
        var lastResult = "Pending";

        for (var i = 0; i < _moves.Length; i++)
        {
            var (row, col) = _moves[i];
            var delta = i % 2 == 0 ? 1 : -1;

            rowCount[row] += delta;
            colCount[col] += delta;
            diag += row == col ? delta : 0;
            antiDiag += row + col == Size - 1 ? delta : 0;

            if (Math.Abs(rowCount[row]) == Size || Math.Abs(colCount[col]) == Size
                || Math.Abs(diag) == Size || Math.Abs(antiDiag) == Size)
            {
                lastResult = delta == 1 ? "A" : "B";
            }
        }

        return lastResult;
    }
}
