using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.XORAfterRangeMultiplicationQueriesI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are XORAfterRangeMultiplicationQueriesISolution's, the same
// methods XORAfterRangeMultiplicationQueriesITests proves correct. nums and queries
// are built once in [GlobalSetup]; both arms take LeetCode's own array shape
// directly, so there is nothing further to hoist.
[MemoryDiagnoser]
public class XORAfterRangeMultiplicationQueriesIBenchmarks
{
    private const int Seed = 3653;
    private const int QueryCount = 200;

    private int[] _nums = [];

    private int[][] _queries = [];
    [Params(100, 1_000)]
    public int NumCount { get; set; }

    [GlobalSetup]
    public void Setup() => (_nums, _queries) = XORAfterRangeMultiplicationQueriesIWorkloads.Build(NumCount, QueryCount, seed: Seed);

    [Benchmark(Baseline = true)]
    public int RangeScan() =>
        XORAfterRangeMultiplicationQueriesISolution.XorAfterQueriesByRangeScan(_nums, _queries);

    [Benchmark]
    public int StridedWalk() =>
        XORAfterRangeMultiplicationQueriesISolution.XorAfterQueriesByStridedWalk(_nums, _queries);
}
