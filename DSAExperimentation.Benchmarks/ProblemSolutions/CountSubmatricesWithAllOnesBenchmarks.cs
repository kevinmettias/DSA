using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountSubmatricesWithAllOnes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountSubmatricesWithAllOnesSolution's, the same methods
// CountSubmatricesWithAllOnesTests proves correct - the O(rows * cols^2) running-minimum
// scan against the O(rows * cols) monotonic-stack reduction to LC 907. The random binary
// matrix is built once in [GlobalSetup] so matrix construction is not charged to either
// measured arm.
[MemoryDiagnoser]
public class CountSubmatricesWithAllOnesBenchmarks
{
    private const int CellValueUpperBoundExclusive = 2;

    private const int RandomSeed = 1;

    [Params(50, 300)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, CellValueUpperBoundExclusive)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RunningMinScan() => CountSubmatricesWithAllOnesSolution.CountByRunningMinScan(_matrix);

    [Benchmark]
    public int MonotonicStackDp() => CountSubmatricesWithAllOnesSolution.CountByMonotonicStackDp(_matrix);
}
