using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindMaximumNonDecreasingArrayLength;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindMaximumNonDecreasingArrayLengthSolution's, the
// same methods FindMaximumNonDecreasingArrayLengthTests proves correct. Random
// values (rather than an already sorted or reverse-sorted array) keep the
// candidate stack the monotonic-stack strategy maintains from collapsing to a
// trivial size, so its binary search is actually exercised against the brute
// force's O(i) scan.
[MemoryDiagnoser]
public class FindMaximumNonDecreasingArrayLengthBenchmarks
{
    private const int MaxValueExclusive = 1_000;
    private const int Seed = 2945;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceDp() => FindMaximumNonDecreasingArrayLengthSolution.FindMaxLengthByBruteForceDp(_nums);

    [Benchmark]
    public int MonotonicStackBinarySearch() =>
        FindMaximumNonDecreasingArrayLengthSolution.FindMaxLengthByMonotonicStackBinarySearch(_nums);
}
