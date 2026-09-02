using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SmallestUniqueSubarray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SmallestUniqueSubarraySolution's, the same methods
// SmallestUniqueSubarrayTests proves correct. A small value bound (relative to
// Length) keeps most windows non-unique at short lengths, so both arms do real
// work across several candidate lengths rather than resolving at length 1.
[MemoryDiagnoser]
public class SmallestUniqueSubarrayBenchmarks
{
    private const int RandomSeed = 3934; // LeetCode problem number
    private const int ValueUpperBound = 50;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => SmallestUniqueSubarraySolution.SmallestUniqueLengthByBruteForce(_nums);

    [Benchmark]
    public int RollingHashBinarySearch() => SmallestUniqueSubarraySolution.SmallestUniqueLengthByRollingHash(_nums);
}
