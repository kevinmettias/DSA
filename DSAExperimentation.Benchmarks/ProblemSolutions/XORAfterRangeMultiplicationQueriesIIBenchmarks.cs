using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.XORAfterRangeMultiplicationQueriesII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are XORAfterRangeMultiplicationQueriesIISolution's, the
// same methods XORAfterRangeMultiplicationQueriesIITests proves correct. nums and
// queries are built once in [GlobalSetup]; both arms take LeetCode's own array
// shape directly, so there is nothing further to hoist.
[MemoryDiagnoser]
public class XORAfterRangeMultiplicationQueriesIIBenchmarks
{
    private const int Seed = 3655;
    private const int QueryCount = 2_000;

    [Params(1_000, 10_000)]
    public int NumCount;

    private int[] _nums = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        (_nums, _queries) = XORAfterRangeMultiplicationQueriesIIWorkloads.Build(NumCount, QueryCount, seed: Seed);
    }

    [Benchmark(Baseline = true)]
    public int StridedWalk() =>
        XORAfterRangeMultiplicationQueriesIISolution.XorAfterQueriesByStridedWalk(_nums, _queries);

    [Benchmark]
    public int SqrtDecomposition() =>
        XORAfterRangeMultiplicationQueriesIISolution.XorAfterQueriesBySqrtDecomposition(_nums, _queries);
}
