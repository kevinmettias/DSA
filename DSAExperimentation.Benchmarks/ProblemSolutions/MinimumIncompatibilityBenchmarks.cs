using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumIncompatibility;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumIncompatibilitySolution's, the same methods
// MinimumIncompatibilityTests proves correct. A shuffled permutation of 1..Length
// split into pairs is always groupable, so neither arm gets to cut the search
// short on an infeasible value - the whole state space is walked, which is exactly
// where re-deriving a remaining-mask per path diverges from caching it once.
[MemoryDiagnoser]
public class MinimumIncompatibilityBenchmarks
{
    private const int GroupSize = 2;

    // LC problem number, reused as the fixed benchmark-data seed.
    private const int RandomSeed = 1681;

    [Params(10, 14)]
    public int Length;

    private int[] _nums = null!;
    private int _groupCount;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
        _groupCount = Length / GroupSize;
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() =>
        MinimumIncompatibilitySolution.MinimumIncompatibilityByUnmemoizedRecursion(_nums, _groupCount);

    [Benchmark]
    public int MemoizedRecursion() =>
        MinimumIncompatibilitySolution.MinimumIncompatibilityByMemoizedRecursion(_nums, _groupCount);
}
