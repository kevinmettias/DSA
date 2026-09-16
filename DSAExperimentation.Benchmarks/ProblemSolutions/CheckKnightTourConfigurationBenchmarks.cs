using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CheckKnightTourConfiguration;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckKnightTourConfigurationSolution's, the same
// methods CheckKnightTourConfigurationTests proves correct. Re-scanning the whole
// n x n board to locate each move value (O(n^4) across all n^2 moves) vs. building
// the move -> cell lookup with a single O(n^2) pass before sweeping consecutive
// pairs (O(n^2) overall). Both Params values are genuine, verified knight's tours
// that begin at the top-left cell (not random shuffles) so no pair fails and
// neither arm gets to exit early - the same "force the full worst-case walk"
// intent TwoSumBenchmarks' unreachable target already uses. n itself is capped by
// the problem's own constraint (n <= 7), so both sizes stay inside LeetCode's real
// input domain.
[MemoryDiagnoser]
public class CheckKnightTourConfigurationBenchmarks
{
    private const int SmallBoard = 5;

    private static readonly int[][] FiveByFiveTour =
    [
        [0, 19, 8, 13, 2],
        [9, 14, 1, 18, 23],
        [20, 7, 22, 3, 12],
        [15, 10, 5, 24, 17],
        [6, 21, 16, 11, 4],
    ];

    private static readonly int[][] SevenBySevenTour =
    [
        [0, 29, 16, 33, 2, 39, 26],
        [17, 32, 1, 28, 25, 34, 3],
        [30, 15, 42, 35, 38, 27, 40],
        [43, 18, 31, 24, 41, 4, 37],
        [14, 21, 44, 47, 36, 7, 10],
        [19, 46, 23, 12, 9, 48, 5],
        [22, 13, 20, 45, 6, 11, 8],
    ];

    private int[][] _grid = [];

    [Params(SmallBoard, 7)]
    public int BoardSize { get; set; }

    [GlobalSetup]
    public void Setup() => _grid = BoardSize == SmallBoard ? FiveByFiveTour : SevenBySevenTour;

    [Benchmark(Baseline = true)]
    public bool IsValidGridByBoardRescan() =>
        CheckKnightTourConfigurationSolution.IsValidGridByBoardRescan(_grid);

    [Benchmark]
    public bool IsValidGridByPositionLookup() =>
        CheckKnightTourConfigurationSolution.IsValidGridByPositionLookup(_grid);
}
