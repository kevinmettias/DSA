using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ZeroOneMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// 01 Matrix (LC 542): an independent BFS per 1 cell vs. one shared multi-source
// BFS. See ZeroOneMatrixSolution for what each strategy does.
[MemoryDiagnoser]
public class ZeroOneMatrixBenchmarks
{
    private const int ZeroCellProbabilityDenominator = 5;

    [Params(10, 25)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, ZeroCellProbabilityDenominator) == 0 ? 0 : 1).ToArray())
            .ToArray();
        _matrix[0][0] = 0;
    }

    [Benchmark(Baseline = true)]
    public int[][] PerCellBfs() => ZeroOneMatrixSolution.UpdateMatrixByPerCellBfs(_matrix);

    [Benchmark]
    public int[][] MultiSourceBfs() => ZeroOneMatrixSolution.UpdateMatrixByMultiSourceBfs(_matrix);
}
