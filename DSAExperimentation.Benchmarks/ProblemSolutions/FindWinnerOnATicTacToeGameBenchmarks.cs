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
    private const string PendingResult = "Pending";
    private const string PlayerAResult = "A";
    private const string PlayerBResult = "B";
    private const int PlayerCount = 2;

    [Params(10, 60)]
    public int Size;

    private (int Row, int Col)[] _moves = null!;

    // A line to scan for a winner: starting cell plus per-step row/column delta.
    private readonly record struct LineScan(int StartRow, int StartCol, int DRow, int DCol);

    // Running per-row/per-column/per-diagonal occupancy tallies for the incremental scan.
    private sealed class LineTallies
    {
        public readonly int[] RowCount;
        public readonly int[] ColCount;
        public int Diag;
        public int AntiDiag;
        public string LastResult = PendingResult;

        public LineTallies(int size)
        {
            RowCount = new int[size];
            ColCount = new int[size];
        }
    }

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
        var lastResult = PendingResult;

        for (var i = 0; i < _moves.Length; i++)
        {
            var (row, col) = _moves[i];
            board[row, col] = i % PlayerCount == 0 ? 1 : -1;

            var winner = ScanAllLines(board, Size);
            if (winner != 0)
            {
                lastResult = winner == 1 ? PlayerAResult : PlayerBResult;
            }
        }

        return lastResult;
    }

    private static int ScanAllLines(int[,] board, int size)
    {
        var rowWinner = ScanRows(board, size);
        if (rowWinner != 0)
        {
            return rowWinner;
        }

        var colWinner = ScanColumns(board, size);
        if (colWinner != 0)
        {
            return colWinner;
        }

        var diagWinner = ScanMainDiagonal(board, size);
        if (diagWinner != 0)
        {
            return diagWinner;
        }

        return ScanAntiDiagonal(board, size);
    }

    private static int ScanRows(int[,] board, int size)
    {
        for (var r = 0; r < size; r++)
        {
            var winner = LineWinner(board, size, new LineScan(r, 0, 0, 1));
            if (winner != 0)
            {
                return winner;
            }
        }

        return 0;
    }

    private static int ScanColumns(int[,] board, int size)
    {
        for (var c = 0; c < size; c++)
        {
            var winner = LineWinner(board, size, new LineScan(0, c, 1, 0));
            if (winner != 0)
            {
                return winner;
            }
        }

        return 0;
    }

    private static int ScanMainDiagonal(int[,] board, int size)
        => LineWinner(board, size, new LineScan(0, 0, 1, 1));

    private static int ScanAntiDiagonal(int[,] board, int size)
        => LineWinner(board, size, new LineScan(0, size - 1, 1, -1));

    private static int LineWinner(int[,] board, int size, LineScan line)
    {
        var first = board[line.StartRow, line.StartCol];

        if (first == 0)
        {
            return 0;
        }

        for (var i = 1; i < size; i++)
        {
            if (board[line.StartRow + (i * line.DRow), line.StartCol + (i * line.DCol)] != first)
            {
                return 0;
            }
        }

        return first;
    }

    [Benchmark]
    public string IncrementalRunningCounts()
    {
        var tallies = new LineTallies(Size);

        for (var i = 0; i < _moves.Length; i++)
        {
            ApplyMove(tallies, _moves[i], i);
        }

        return tallies.LastResult;
    }

    private void ApplyMove(LineTallies tallies, (int Row, int Col) move, int moveIndex)
    {
        var (row, col) = move;
        var delta = moveIndex % PlayerCount == 0 ? 1 : -1;

        tallies.RowCount[row] += delta;
        tallies.ColCount[col] += delta;
        tallies.Diag += row == col ? delta : 0;
        tallies.AntiDiag += row + col == Size - 1 ? delta : 0;

        if (Math.Abs(tallies.RowCount[row]) == Size || Math.Abs(tallies.ColCount[col]) == Size
            || Math.Abs(tallies.Diag) == Size || Math.Abs(tallies.AntiDiag) == Size)
        {
            tallies.LastResult = delta == 1 ? PlayerAResult : PlayerBResult;
        }
    }
}
