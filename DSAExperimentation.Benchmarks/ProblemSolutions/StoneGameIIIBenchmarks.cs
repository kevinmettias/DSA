using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Stone Game III (LC 1406): plain un-memoized minimax recursion over index -
// exponential (tribonacci-shaped growth), since the same index recurs
// through many different take-1/2/3 pick sequences reaching it - vs. this
// repo's own Memoizer<TState,TResult> caching that index, the identical
// shape StoneGameBenchmarks/StoneGameIIBenchmarks already use. PileCount is
// kept modest for the same reason those benchmarks document: the
// un-memoized baseline's blowup is real, just slower-growing here than LC
// 877's binary branching since each state only has up to 3 children instead
// of an M-bounded window.
[MemoryDiagnoser]
public class StoneGameIIIBenchmarks
{
    private const int RandomSeed = 1406; // LC 1406
    private const int MinStoneValue = -50;
    private const int StoneValueUpperBound = 100;
    private const int MaxTakePerTurn = 3;

    [Params(20, 24)]
    public int PileCount;

    private int[] _stoneValue = null!;
    private Func<int, int>? _best;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stoneValue = Enumerable.Range(0, PileCount).Select(_ => random.Next(MinStoneValue, StoneValueUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => Best(0);

    private int Best(int index) => ComputeBest(index, _best ??= Best);

    [Benchmark]
    public int MemoizedRecursion() => Memoizer.Memoize<int, int>(0, BestMemoized);

    private int BestMemoized(int index, Func<int, int> bestFrom) => ComputeBest(index, bestFrom);

    private int ComputeBest(int index, Func<int, int> bestFrom)
    {
        if (index >= PileCount)
        {
            return 0;
        }

        var result = int.MinValue;
        var takenSum = 0;
        for (var take = 1; take <= MaxTakePerTurn && index + take <= PileCount; take++)
        {
            takenSum += _stoneValue[index + take - 1];
            result = Math.Max(result, takenSum - bestFrom(index + take));
        }

        return result;
    }
}
