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
    // LC 289.
    private const int RandomSeed = 289;
    private const int LiveCellExclusiveBound = 2;
    private const int BirthNeighborCount = 3;
    private const int SurvivalNeighborCount = 2;

    [Params(50, 300)]
    public int Size;

    private int[][] _board = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _board = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, LiveCellExclusiveBound)).ToArray())
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
                var liveNeighbors = CountLiveNeighborsFromSnapshot(snapshot, new GridPosition(rows, cols, r, c));
                board[r][c] = liveNeighbors == BirthNeighborCount
                    || (liveNeighbors == SurvivalNeighborCount && snapshot[r][c] == 1) ? 1 : 0;
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

        var originallyLive = BuildOriginallyLive(board, rows, cols);
        ApplyLifeRulesFromSet(board, originallyLive, rows, cols);

        return board;
    }

    private static Set<int> BuildOriginallyLive(int[][] board, int rows, int cols)
    {
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

        return originallyLive;
    }

    private static void ApplyLifeRulesFromSet(int[][] board, Set<int> originallyLive, int rows, int cols)
    {
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var liveNeighbors = CountLiveNeighborsFromSet(originallyLive, new GridPosition(rows, cols, r, c));
                board[r][c] = liveNeighbors == BirthNeighborCount
                    || (liveNeighbors == SurvivalNeighborCount && originallyLive.Has(r * cols + c)) ? 1 : 0;
            }
        }
    }

    private static int CountLiveNeighborsFromSnapshot(int[][] snapshot, GridPosition position)
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

                var r = position.Row + dr;
                var c = position.Col + dc;

                if (r >= 0 && r < position.Rows && c >= 0 && c < position.Cols && snapshot[r][c] == 1)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static int CountLiveNeighborsFromSet(Set<int> originallyLive, GridPosition position)
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

                var r = position.Row + dr;
                var c = position.Col + dc;

                if (r >= 0 && r < position.Rows && c >= 0 && c < position.Cols && originallyLive.Has(r * position.Cols + c))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static int[][] Clone(int[][] matrix)
        => matrix.Select(row => (int[])row.Clone()).ToArray();

    private readonly record struct GridPosition(int Rows, int Cols, int Row, int Col);
}
