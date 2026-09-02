using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Stone Game VIII (LC 1872): plain un-memoized minimax recursion over the boundary
// chain - exponential, since Best(boundary+1) recurs through every larger boundary
// that in turn recurs through every boundary past it again - vs. this repo's own
// Memoizer<TState,TResult> caching each boundary's result, the identical shape
// StoneGameVIIBenchmarks already uses for LC 1690's own (left,right) minimax
// recurrence, just over a single-index state instead of a pair. PileCount is kept
// modest for the same reason StoneGameVIIBenchmarks documents: the un-memoized
// baseline's blowup is real.
[MemoryDiagnoser]
public class StoneGameVIIIBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1872;

    private const int StoneValueBound = 100;

    [Params(22, 26)]
    public int PileCount;

    private long[] _prefix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var stones = Enumerable.Range(0, PileCount).Select(_ => random.Next(-StoneValueBound, StoneValueBound)).ToArray();
        _prefix = new long[PileCount];
        _prefix[0] = stones[0];

        for (var i = 1; i < PileCount; i++)
        {
            _prefix[i] = _prefix[i - 1] + stones[i];
        }
    }

    [Benchmark(Baseline = true)]
    public long UnmemoizedRecursion() => Best(1);

    private long Best(int boundary)
    {
        if (boundary == PileCount - 1)
        {
            return _prefix[boundary];
        }

        var takeHere = _prefix[boundary] - Best(boundary + 1);
        return Math.Max(Best(boundary + 1), takeHere);
    }

    [Benchmark]
    public long MemoizedRecursion() => Memoizer.Memoize<int, long>(1, BestMemoized);

    private long BestMemoized(int boundary, Func<int, long> best)
    {
        if (boundary == PileCount - 1)
        {
            return _prefix[boundary];
        }

        var takeHere = _prefix[boundary] - best(boundary + 1);
        return Math.Max(best(boundary + 1), takeHere);
    }
}
