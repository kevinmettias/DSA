using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastKSolution's, the same
// methods MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastKTests proves
// correct. Mirrors MaximumBalancedSubsequenceSumBenchmarks' shape - the textbook
// O(n^2) pairwise scan against an O(n log n) sweep through a repo index
// structure, this time two SegmentTree<long,MaxOperation<long>> trees instead
// of one.
[MemoryDiagnoser]
public class MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastKBenchmarks
{
    private const int RandomSeed = 3915; // LeetCode problem number
    private const int ValueUpperBound = 100_000;

    private int[] _nums = [];

    private int _k;
    [Params(2_000, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueUpperBound)).ToArray();
        _k = Math.Max(1, Length / 10);
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() =>
        MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastKSolution.MaxAlternatingSumByBruteForce(_nums, _k);

    [Benchmark]
    public long SegmentTreeSweep() =>
        MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastKSolution.MaxAlternatingSumBySegmentTree(_nums, _k);
}
