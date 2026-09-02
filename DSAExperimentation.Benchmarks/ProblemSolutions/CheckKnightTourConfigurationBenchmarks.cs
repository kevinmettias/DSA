using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Check Knight Tour Configuration (LC 2596): re-scanning the whole n x n board to
// locate each order value (O(n^4) total across all n^2 orders) vs. building the
// order -> cell lookup with a single O(n^2) pass before sweeping consecutive pairs
// (O(n^2) overall). No repo primitive applies to either arm - see
// CheckKnightTourConfigurationTests.cs's own doc comment for why Grid/GridChildren
// isn't a genuine fit. Both Params values are genuine, verified knight's tours
// (not random shuffles) so every consecutive pair actually is a valid knight move
// and neither arm gets to exit early - the same "force the full worst-case walk"
// intent TwoSumBenchmarks' unreachable target already uses. n itself is capped by
// the problem's own constraint (n <= 7), so both sizes stay inside LeetCode's real
// input domain.
[MemoryDiagnoser]
public class CheckKnightTourConfigurationBenchmarks
{
    private static readonly (int DRow, int DCol)[] KnightOffsets =
        [(1, 2), (1, -2), (-1, 2), (-1, -2), (2, 1), (2, -1), (-2, 1), (-2, -1)];

    private static readonly int[][] FiveByFiveTour =
    [
        [20, 3, 12, 9, 22],
        [13, 8, 21, 4, 11],
        [2, 19, 10, 23, 16],
        [7, 14, 17, 0, 5],
        [18, 1, 6, 15, 24],
    ];

    private static readonly int[][] SevenBySevenTour =
    [
        [22, 37, 0, 47, 20, 39, 26],
        [1, 30, 21, 38, 25, 42, 19],
        [36, 23, 46, 29, 48, 27, 40],
        [31, 2, 35, 24, 41, 18, 43],
        [8, 5, 32, 45, 28, 15, 12],
        [3, 34, 7, 10, 13, 44, 17],
        [6, 9, 4, 33, 16, 11, 14],
    ];

    [Params(5, 7)]
    public int N;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup() => _grid = N == 5 ? FiveByFiveTour : SevenBySevenTour;

    [Benchmark(Baseline = true)]
    public bool RescanBoardPerOrder() => ValidTourByRescanning(_grid);

    private static bool ValidTourByRescanning(int[][] grid)
    {
        var n = grid.Length;

        for (var order = 0; order < (n * n) - 1; order++)
        {
            var from = FindOrder(grid, order);
            var to = FindOrder(grid, order + 1);

            if (!IsKnightMove(from, to))
            {
                return false;
            }
        }

        return true;
    }

    private static (int Row, int Col) FindOrder(int[][] grid, int order)
    {
        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid.Length; col++)
            {
                if (grid[row][col] == order)
                {
                    return (row, col);
                }
            }
        }

        throw new InvalidOperationException();
    }

    [Benchmark]
    public bool SinglePassPositionLookup() => ValidTourByPositionLookup(_grid);

    private static bool ValidTourByPositionLookup(int[][] grid)
    {
        var n = grid.Length;
        var positionByOrder = new (int Row, int Col)[n * n];

        for (var row = 0; row < n; row++)
        {
            for (var col = 0; col < n; col++)
            {
                positionByOrder[grid[row][col]] = (row, col);
            }
        }

        for (var order = 0; order < (n * n) - 1; order++)
        {
            if (!IsKnightMove(positionByOrder[order], positionByOrder[order + 1]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsKnightMove((int Row, int Col) from, (int Row, int Col) to)
    {
        foreach (var (dRow, dCol) in KnightOffsets)
        {
            if (from.Row + dRow == to.Row && from.Col + dCol == to.Col)
            {
                return true;
            }
        }

        return false;
    }
}
