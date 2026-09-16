using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SearchA2DMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SearchA2DMatrixSolution's, the same methods
// SearchA2DMatrixTests proves correct. The target is fixed to the workload's
// largest value, so both arms search for a value guaranteed present.
[MemoryDiagnoser]
public class SearchA2DMatrixBenchmarks
{
    private int[][] _matrix = [];

    [Params(40, 100)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var value = 0;
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => value++).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool HasTargetByLinearScan() =>
        SearchA2DMatrixSolution.HasTargetByLinearScan(_matrix, Size * Size - 1);

    [Benchmark]
    public bool HasTargetByBinarySearch() =>
        SearchA2DMatrixSolution.HasTargetByBinarySearch(_matrix, Size * Size - 1);
}
