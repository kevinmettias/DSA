using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SmallestUniqueSubarray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SmallestUniqueSubarraySolution's, the same methods
// SmallestUniqueSubarraySolutionTests proves correct. A small value bound (relative to
// Length) keeps most windows non-unique at short lengths, so both arms do real
// work across several candidate lengths rather than resolving at length 1.
public class SmallestUniqueSubarrayBenchmarks
{
    private const int RandomSeed = 3934; // LeetCode problem number
    private const int ValueUpperBound = 50;

    private int[] _nums = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = SeededDraws.Values(Length, 1, ValueUpperBound, random);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => SmallestUniqueSubarraySolution.SmallestUniqueLengthByBruteForce(_nums);

    [Benchmark]
    public int RollingHashBinarySearch() => SmallestUniqueSubarraySolution.SmallestUniqueLengthByRollingHash(_nums);
}
