using DSAExperimentation.LeetCode.PaintingAGridWithThreeDifferentColors;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PaintingAGridWithThreeDifferentColorsSolution's, the
// same methods PaintingAGridWithThreeDifferentColorsSolutionTests proves correct. Neither
// arm takes prepared input - the whole input is two integers, so there is nothing for
// a [GlobalSetup] to build.
//
// Sizes are per arm, counted in columns. The 3^(rows*columns) brute force stops at 4
// columns; the column-pattern DP runs on to LeetCode's real n <= 1000 with no change.
// The two are compared at the sizes both run.
public class PaintingAGridWithThreeDifferentColorsBenchmarks
{
    private const int Rows = 3;

    public static IEnumerable<int> BaselineSizes => [3, 4];

    public static IEnumerable<int> ColumnPatternSizes => [.. BaselineSizes, 100, 1_000];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int BruteForceFullGrid(int columns) =>
        PaintingAGridWithThreeDifferentColorsSolution.ColorTheGridByBruteForce(Rows, columns);

    [Benchmark]
    [ArgumentsSource(nameof(ColumnPatternSizes))]
    public int ColumnPatternDynamicProgramming(int columns) =>
        PaintingAGridWithThreeDifferentColorsSolution.ColorTheGridByColumnPatternDynamicProgramming(Rows, columns);
}
