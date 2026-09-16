using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.MaximumScoreOfAGoodSubarray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumScoreOfAGoodSubarraySolution's, the same
// methods MaximumScoreOfAGoodSubarrayTests proves correct - the O(n^2) baseline
// that re-scans every window anchored at the required index against the two
// monotonic-increasing Stack<int> sweeps that compute each index's nearest-smaller
// boundaries in one O(n) pass per direction. LeetCode's own input here is already
// the prepared input, so [GlobalSetup] only sizes and seeds the array; there is no
// separate hoisted overload to hand it to.
[MemoryDiagnoser]
public class MaximumScoreOfAGoodSubarrayBenchmarks
{
    private const int RandomSeed = 3;
    private const int MaxNumValue = 20_000;

    private int[] _nums = [];

    private int _requiredIndex;
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxNumValue)).ToArray();
        _requiredIndex = Length / AlgorithmConstants.HalvingFactor;
    }

    [Benchmark(Baseline = true)]
    public int BruteForceExpand() =>
        MaximumScoreOfAGoodSubarraySolution.MaximumScoreByBruteForceExpand(_nums, _requiredIndex);

    [Benchmark]
    public int MonotonicStackBoundaries() =>
        MaximumScoreOfAGoodSubarraySolution.MaximumScoreByMonotonicStackBoundaries(_nums, _requiredIndex);
}
