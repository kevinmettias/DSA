using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Game of Life (LC 289): a full-board-copy baseline (clones the board into a second
// snapshot so counting neighbors never reads an already-updated cell, O(rows*cols)
// extra space) vs. this repo's own Set<int> snapshotting only which cells started
// live (row*cols+col encoded) - O(live cells) extra space, the same "sparse Set
// instead of a full second copy" move SetMatrixZeroesBenchmarks already makes. Both
// clone the shared fixture first so mutating one iteration's result never corrupts
// the next.
[MemoryDiagnoser]
public class GameOfLifeBenchmarks
{
    [Params(50, 300)]
    public int Size;

    private int[][] _board = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(289);
        _board = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, 2)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[][] FullBoardCopy()
    {
        var board = Clone(_board);
        var snapshot = Clone(board);
        var rows = board.Length;
        var cols = board[0].Length;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var liveNeighbors = CountLiveNeighborsFromSnapshot(snapshot, rows, cols, r, c);
                board[r][c] = liveNeighbors == 3 || (liveNeighbors == 2 && snapshot[r][c] == 1) ? 1 : 0;
            }
        }

        return board;
    }

    [Benchmark]
    public int[][] SetSnapshot()
    {
        var board = Clone(_board);
        var rows = board.Length;
        var cols = board[0].Length;
        var originallyLive = new Set<int>();

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (board[r][c] == 1)
                {
                    originallyLive.TryAdd(r * cols + c);
                }
            }
        }

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var liveNeighbors = CountLiveNeighborsFromSet(originallyLive, rows, cols, r, c);
                board[r][c] = liveNeighbors == 3 || (liveNeighbors == 2 && originallyLive.Has(r * cols + c)) ? 1 : 0;
            }
        }

        return board;
    }

    private static int CountLiveNeighborsFromSnapshot(int[][] snapshot, int rows, int cols, int row, int col)
    {
        var count = 0;

        for (var dr = -1; dr <= 1; dr++)
        {
            for (var dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0)
                {
                    continue;
                }

                var r = row + dr;
                var c = col + dc;

                if (r >= 0 && r < rows && c >= 0 && c < cols && snapshot[r][c] == 1)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static int CountLiveNeighborsFromSet(Set<int> originallyLive, int rows, int cols, int row, int col)
    {
        var count = 0;

        for (var dr = -1; dr <= 1; dr++)
        {
            for (var dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0)
                {
                    continue;
                }

                var r = row + dr;
                var c = col + dc;

                if (r >= 0 && r < rows && c >= 0 && c < cols && originallyLive.Has(r * cols + c))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static int[][] Clone(int[][] matrix)
        => matrix.Select(row => (int[])row.Clone()).ToArray();
}
