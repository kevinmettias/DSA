using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Available Captures for Rook (LC 999): scanning every cell of the whole board
// for a pawn that happens to share the rook's row or column, path-checking each
// one found (O(rows*cols) just for the scan) vs. the direct four-direction ray
// walk that only ever visits cells the rook could actually reach, stopping at the
// first non-empty one (O(rows+cols)). No repo primitive applies to either shape -
// see AvailableCapturesForRookTests.cs's own doc comment for why Grid/GridChildren
// isn't a genuine fit.
[MemoryDiagnoser]
public class AvailableCapturesForRookBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Params(50, 500)]
    public int Size;

    private char[][] _board = null!;
    private (int Row, int Col) _rook;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _board = Enumerable.Range(0, Size).Select(_ => Enumerable.Repeat('.', Size).ToArray()).ToArray();
        _rook = (Size / 2, Size / 2);
        _board[_rook.Row][_rook.Col] = 'R';

        for (var col = 0; col < Size; col++)
        {
            if (col != _rook.Col && random.Next(4) == 0)
            {
                _board[_rook.Row][col] = 'p';
            }
        }

        for (var row = 0; row < Size; row++)
        {
            if (row != _rook.Row && random.Next(4) == 0)
            {
                _board[row][_rook.Col] = 'p';
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int FullBoardScan() => CountViaFullBoardScan(_board, _rook);

    [Benchmark]
    public int DirectRayWalk() => CountViaRayWalk(_board, _rook);

    private static int CountViaFullBoardScan(char[][] board, (int Row, int Col) rook)
    {
        var captures = 0;

        for (var row = 0; row < board.Length; row++)
        {
            for (var col = 0; col < board[0].Length; col++)
            {
                if (board[row][col] != 'p' || (row != rook.Row && col != rook.Col))
                {
                    continue;
                }

                if (IsPathClear(board, rook.Row, rook.Col, row, col))
                {
                    captures++;
                }
            }
        }

        return captures;
    }

    private static bool IsPathClear(char[][] board, int fromRow, int fromCol, int toRow, int toCol)
    {
        var dRow = Math.Sign(toRow - fromRow);
        var dCol = Math.Sign(toCol - fromCol);
        var row = fromRow + dRow;
        var col = fromCol + dCol;

        while (row != toRow || col != toCol)
        {
            if (board[row][col] != '.')
            {
                return false;
            }

            row += dRow;
            col += dCol;
        }

        return true;
    }

    private static int CountViaRayWalk(char[][] board, (int Row, int Col) rook)
    {
        var captures = 0;

        foreach (var (dRow, dCol) in Directions)
        {
            var row = rook.Row + dRow;
            var col = rook.Col + dCol;

            while (IsInBounds(board, row, col) && board[row][col] == '.')
            {
                row += dRow;
                col += dCol;
            }

            if (IsInBounds(board, row, col) && board[row][col] == 'p')
            {
                captures++;
            }
        }

        return captures;
    }

    private static bool IsInBounds(char[][] board, int row, int col) =>
        row >= 0 && row < board.Length && col >= 0 && col < board[0].Length;
}
