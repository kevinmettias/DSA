using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.LeetCode.StoneGameVIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StoneGameVIIISolution's, the same methods
// StoneGameVIIITests proves correct - plain un-memoized minimax over the boundary
// chain, exponential because every boundary re-walks the whole tail behind it,
// against this repo's own Memoizer<TState,TResult> caching each boundary's result.
// The prefix table is built once in [GlobalSetup] by the solution's own
// BuildPrefixSums and handed to each arm's hoisted overload, so only the recursion
// is measured. PileCount is kept modest for the same reason StoneGameVIIBenchmarks
// documents: the un-memoized baseline's blowup is real.
[MemoryDiagnoser]
public class StoneGameVIIIBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1872;

    private const int StoneValueBound = 100;

    [Params(22, 26)]
    public int PileCount;

    private ArraySequence<long> _prefix;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var stones = Enumerable.Range(0, PileCount).Select(_ => random.Next(-StoneValueBound, StoneValueBound)).ToArray();
        _prefix = StoneGameVIIISolution.BuildPrefixSums(stones);
    }

    [Benchmark(Baseline = true)]
    public long UnmemoizedRecursion() => StoneGameVIIISolution.MaxScoreDifferenceByUnmemoizedRecursion(_prefix);

    [Benchmark]
    public long MemoizedRecursion() => StoneGameVIIISolution.MaxScoreDifferenceByMemoizedRecursion(_prefix);
}
