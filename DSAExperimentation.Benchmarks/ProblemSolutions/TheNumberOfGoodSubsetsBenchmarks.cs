using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TheNumberOfGoodSubsets;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TheNumberOfGoodSubsetsSolution's, the same methods
// TheNumberOfGoodSubsetsTests proves correct - the unmemoized (candidateIndex,
// usedPrimeMask) recursion against the identical recursion routed through this repo's
// own Memoizer. Each arm is handed the prepared GoodSubsetCandidates its hoisted
// overload takes, so reducing nums to squarefree candidates is charged to [GlobalSetup]
// rather than to the search being measured.
//
// The candidate list is ~18 values whatever Length is - the problem's own constraints
// fix it - so Length varies only how heavily each candidate is weighted, not the
// recursion's shape.
[MemoryDiagnoser]
public class TheNumberOfGoodSubsetsBenchmarks
{
    // LC problem number, reused as the deterministic value seed.
    private const int RandomSeed = 1994;

    private const int MinValue = 1;
    private const int ValueUpperBoundExclusive = 31;

    private GoodSubsetCandidates _candidates = null!;

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var nums = Enumerable.Range(0, Length)
            .Select(_ => random.Next(MinValue, ValueUpperBoundExclusive))
            .ToArray();

        _candidates = GoodSubsetCandidates.Build(nums);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() =>
        TheNumberOfGoodSubsetsSolution.NumberOfGoodSubsetsByBruteForceRecursion(_candidates);

    [Benchmark]
    public int MemoizedRecursion() =>
        TheNumberOfGoodSubsetsSolution.NumberOfGoodSubsetsByMemoizedRecursion(_candidates);
}
