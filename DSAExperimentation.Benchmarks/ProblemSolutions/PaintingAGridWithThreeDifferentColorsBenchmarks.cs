using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PaintingAGridWithThreeDifferentColors;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PaintingAGridWithThreeDifferentColorsSolution's, the
// same methods PaintingAGridWithThreeDifferentColorsTests proves correct. Rows and
// Columns stay small enough for the 3^(rows*columns) brute force to remain
// tractable; the column-pattern DP alone scales to LeetCode's real n <= 1000 with
// no change. Neither arm takes prepared input - the whole input is two integers, so
// there is nothing for a [GlobalSetup] to build.
[MemoryDiagnoser]
public class PaintingAGridWithThreeDifferentColorsBenchmarks
{
    private const int Rows = 3;

    [Params(3, 4)]
    public int Columns;

    [Benchmark(Baseline = true)]
    public int BruteForceFullGrid() =>
        PaintingAGridWithThreeDifferentColorsSolution.ColorTheGridByBruteForce(Rows, Columns);

    [Benchmark]
    public int ColumnPatternDynamicProgramming() =>
        PaintingAGridWithThreeDifferentColorsSolution.ColorTheGridByColumnPatternDynamicProgramming(Rows, Columns);
}
