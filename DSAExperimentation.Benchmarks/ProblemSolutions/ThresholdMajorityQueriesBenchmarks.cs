using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ThresholdMajorityQueries;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ThresholdMajorityQueriesSolution's, the same methods
// ThresholdMajorityQueriesTests proves correct. The block-mode arm is handed a
// prebuilt ThresholdMajorityBlockIndex via its hoisted overload, so index
// construction is charged to [GlobalSetup] rather than to the batch of queries
// being measured; the brute-force arm has no comparable prebuild step, so it takes
// nums directly.
[MemoryDiagnoser]
public class ThresholdMajorityQueriesBenchmarks
{
    // LC problem number, reused as the deterministic value/query seed.
    private const int WorkloadSeed = 3636;

    private int[] _nums = [];

    private int[][] _queries = [];
    private ThresholdMajorityBlockIndex _index = null!;
    [Params(500, 2000)]
    public int ElementCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _nums = ThresholdMajorityQueriesWorkloads.BuildNums(ElementCount, seed: WorkloadSeed);
        _queries = ThresholdMajorityQueriesWorkloads.BuildQueries(ElementCount, seed: WorkloadSeed);
        _index = ThresholdMajorityBlockIndex.Build(_nums);
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() => ThresholdMajorityQueriesSolution.SubarrayMajorityByBruteForce(_nums, _queries);

    [Benchmark]
    public int[] BlockMode() => ThresholdMajorityQueriesSolution.SubarrayMajorityByBlockMode(_index, _queries);
}
