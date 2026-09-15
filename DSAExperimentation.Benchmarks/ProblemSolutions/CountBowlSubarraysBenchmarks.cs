using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountBowlSubarrays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountBowlSubarraysSolution's, the same methods
// CountBowlSubarraysTests proves correct. Neither strategy needs anything
// prepared beyond the array itself, so [GlobalSetup] only charges workload
// construction.
[MemoryDiagnoser]
public class CountBowlSubarraysBenchmarks
{
    private const int Seed = 3676;

    private int[] _nums = [];

    [Params(200, 1000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = CountBowlSubarraysWorkloads.BuildNums(Size, Seed);

    [Benchmark(Baseline = true)]
    public int PairScan() => CountBowlSubarraysSolution.CountBowlsByPairScan(_nums);

    [Benchmark]
    public int MonotonicStack() => CountBowlSubarraysSolution.CountBowlsByMonotonicStack(_nums);
}
