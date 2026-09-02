using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximumSolution's, the same
// methods FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximumTests proves
// correct. Nothing needs hoisting into [GlobalSetup] beyond the array itself -
// unlike a graph problem, neither strategy has a separate prepared-input shape to
// be handed.
[MemoryDiagnoser]
public class FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximumBenchmarks
{
    private const int Seed = 3113;
    private const int MaxValueExclusive = 1_000_000_001; // LC bounds nums[i] to [1, 1e9]

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            _nums[i] = random.Next(1, MaxValueExclusive);
        }
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() =>
        FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximumSolution.CountByBruteForce(_nums);

    [Benchmark]
    public long MonotonicStack() =>
        FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximumSolution.CountByMonotonicStack(_nums);
}
