using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StoneGameVII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StoneGameVIISolution's, the same methods
// StoneGameVIITests proves correct. UnmemoizedRecursion is plain minimax recursion
// over (left, right) bounds - exponential, since the same sub-range recurs through
// many different removal orders - against this repo's own Memoizer<TState,TResult>
// caching that exact pair, the identical shape StoneGameVBenchmarks already uses for
// LC 1563. PileCount is kept modest for the same reason StoneGameBenchmarks
// documents: the un-memoized baseline's blowup is real.
[MemoryDiagnoser]
public class StoneGameVIIBenchmarks
{
    // LC problem number, used as the deterministic seed for stone-value generation.
    private const int RandomSeed = 1690;

    private const int StoneValueUpperBoundExclusive = 100;

    private int[] _stones = [];

    [Params(22, 26)]
    public int PileCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stones = Enumerable.Range(0, PileCount)
            .Select(_ => random.Next(1, StoneValueUpperBoundExclusive))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => StoneGameVIISolution.MaxScoreDifferenceByUnmemoizedRecursion(_stones);

    [Benchmark]
    public int MemoizedRecursion() => StoneGameVIISolution.MaxScoreDifferenceByMemoizedRecursion(_stones);
}
