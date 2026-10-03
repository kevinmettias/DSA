using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountSubmatricesWithAllOnes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountSubmatricesWithAllOnesSolution's, the same methods
// CountSubmatricesWithAllOnesSolutionTests proves correct - the O(rows * cols^2) running-minimum
// scan against the O(rows * cols) monotonic-stack reduction to LC 907. The random binary
// matrix is built once in [GlobalSetup] so matrix construction is not charged to either
// measured arm.
public class CountSubmatricesWithAllOnesBenchmarks
{
    private const int CellValueUpperBoundExclusive = 2;

    private const int RandomSeed = 1;

    private int[][] _matrix = [];

    [Params(50, 300)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => SeededDraws.Values(Size, 0, CellValueUpperBoundExclusive, random))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RunningMinScan() => CountSubmatricesWithAllOnesSolution.CountByRunningMinScan(_matrix);

    [Benchmark]
    public int MonotonicStackDp() => CountSubmatricesWithAllOnesSolution.CountByMonotonicStackDp(_matrix);
}
