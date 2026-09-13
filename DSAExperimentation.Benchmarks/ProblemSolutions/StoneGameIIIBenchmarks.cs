using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StoneGameIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StoneGameIIISolution's, the same methods
// StoneGameIIITests proves correct. UnmemoizedRecursion is plain minimax over the
// index - exponential (tribonacci-shaped growth), since the same index recurs
// through many different take-1/2/3 pick sequences reaching it - against this repo's
// own Memoizer<TState,TResult> caching that index, the identical shape
// StoneGameBenchmarks/StoneGameIIBenchmarks already use. PileCount is kept modest
// for the same reason those benchmarks document: the un-memoized baseline's blowup
// is real, just slower-growing here than LC 877's binary branching since each state
// only has up to 3 children instead of an M-bounded window.
//
// Both arms now return LC 1406's actual answer - the winner's name - rather than the
// raw score difference the previous arms reported. That is one sign test on top of
// the same recurrence, identical in both arms, so what the comparison isolates is
// still memoization alone.
[MemoryDiagnoser]
public class StoneGameIIIBenchmarks
{
    // LC problem number, used as the deterministic seed for stone-value generation.
    private const int RandomSeed = 1406;

    // LC 1406 allows negative stone values; these bracket the generated range.
    private const int MinStoneValue = -50;
    private const int StoneValueUpperBound = 100;

    [Params(20, 24)]
    public int PileCount;

    private int[] _stoneValue = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stoneValue = Enumerable.Range(0, PileCount)
            .Select(_ => random.Next(MinStoneValue, StoneValueUpperBound))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public string UnmemoizedRecursion() => StoneGameIIISolution.WinnerByUnmemoizedRecursion(_stoneValue);

    [Benchmark]
    public string MemoizedRecursion() => StoneGameIIISolution.WinnerByMemoizedRecursion(_stoneValue);
}
