using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SetMatrixZeroes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SetMatrixZeroesSolution's, the same methods
// SetMatrixZeroesTests proves correct. Each iteration clones the pristine
// matrix before zeroing, since the solution mutates in place and
// [GlobalSetup] runs once per benchmark, not once per invocation.
[MemoryDiagnoser]
public class SetMatrixZeroesBenchmarks
{
    private const int ZeroProbabilityDenominator = 100;
    private const int MaxCellValue = 1_000;

    [Params(50, 300)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size)
                .Select(_ => random.Next(0, ZeroProbabilityDenominator) == 0 ? 0 : random.Next(1, MaxCellValue))
                .ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[][] CopyAndScan()
    {
        var matrix = Clone(_matrix);
        SetMatrixZeroesSolution.SetZeroesByCopyAndScan(matrix);
        return matrix;
    }

    [Benchmark]
    public int[][] RowColumnSets()
    {
        var matrix = Clone(_matrix);
        SetMatrixZeroesSolution.SetZeroesByRowColumnSets(matrix);
        return matrix;
    }

    private static int[][] Clone(int[][] matrix) =>
        matrix.Select(row => (int[])row.Clone()).ToArray();
}
