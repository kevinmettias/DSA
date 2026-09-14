using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.KDivisibleElementsSubarrays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KDivisibleElementsSubarraysSolution's, the same
// methods KDivisibleElementsSubarraysTests proves agree. They enumerate the same
// k-truncated candidate set, so the measurement isolates the dedupe container - a
// BCL HashSet<string> against this repo's HashMap-backed Set<string>.
//
// The workload is the LeetCode input shape itself, so building it in [GlobalSetup]
// already keeps array construction off the measured methods.
[MemoryDiagnoser]
public class KDivisibleElementsSubarraysBenchmarks
{
    private const int RandomSeed = 2261; // LC problem number
    private const int MaxValueExclusive = 200;
    private const int DivisorP = 3;
    private const int MaxDivisibleCount = 5;

    [Params(50, 200)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int HashSetDeduped() =>
        KDivisibleElementsSubarraysSolution.CountDistinctByHashSetDedupe(_nums, MaxDivisibleCount, DivisorP);

    [Benchmark]
    public int RepoSetDeduped() =>
        KDivisibleElementsSubarraysSolution.CountDistinctBySetDedupe(_nums, MaxDivisibleCount, DivisorP);
}
