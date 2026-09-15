using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumSizeSubarraySum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumSizeSubarraySumSolution's, the same methods
// MinimumSizeSubarraySumTests proves correct - the brute-force O(n^2)
// every-subarray scan against the O(n log n) prefix-sum + BinarySearch.LowerBound
// approach, genuinely composing this repo's own BinarySearch.LowerBound over an
// ArraySequence<int> witness. Target is set one above the array's own total sum so
// neither strategy ever early-exits on a found window, forcing both through their
// real worst-case cost. Only the raw array/target - LeetCode's own input shape -
// is prepared in [GlobalSetup]; each strategy's own prefix-sum/sequence
// construction stays inside the measured method, unchanged from the original.
[MemoryDiagnoser]
public class MinimumSizeSubarraySumBenchmarks
{
    private const int RandomSeed = 7;
    private const int MaxElementValueExclusive = 100;

    private int[] _nums = [];

    private int _target;
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxElementValueExclusive)).ToArray();
        _target = _nums.Sum() + 1;
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => MinimumSizeSubarraySumSolution.MinLengthByBruteForce(_target, _nums);

    [Benchmark]
    public int BinarySearchPrefixSum() =>
        MinimumSizeSubarraySumSolution.MinLengthByBinarySearchPrefixSum(_target, _nums);
}
