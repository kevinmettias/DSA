using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Stone Game V (LC 1563): plain un-memoized interval recursion over (left,right) -
// the same (i,j) range gets re-entered from many different parent splits (e.g.
// Best(0,3) is reachable both directly and as the "left" half of several larger
// ranges), so call count grows far past the O(n^2) distinct-range count once un-
// cached (measured ~9.1M calls at PileCount=220 vs. only 24,310 distinct (i,j)
// states) - vs. this repo's own Memoizer<TState,TResult> caching each (Left,Right)
// range exactly once, the identical shape StoneGameIIIBenchmarks already uses for
// LC 1406, just keyed on a range instead of a single index.
[MemoryDiagnoser]
public class StoneGameVBenchmarks
{
    private const int RandomSeed = 1563;
    private const int MaxStoneValue = 100;

    [Params(120, 200)]
    public int PileCount;

    private int[] _stoneValue = null!;
    private int[] _prefix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stoneValue = Enumerable.Range(0, PileCount).Select(_ => random.Next(1, MaxStoneValue)).ToArray();
        _prefix = new int[PileCount + 1];

        for (var i = 0; i < PileCount; i++)
        {
            _prefix[i + 1] = _prefix[i] + _stoneValue[i];
        }
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => Best(0, PileCount - 1);

    private int Best(int left, int right)
    {
        if (left == right)
        {
            return 0;
        }

        var result = 0;

        for (var mid = left; mid < right; mid++)
        {
            result = BestForSplit(left, right, mid, result);
        }

        return result;
    }

    private int BestForSplit(int left, int right, int mid, int result)
    {
        var leftSum = _prefix[mid + 1] - _prefix[left];
        var rightSum = _prefix[right + 1] - _prefix[mid + 1];

        if (leftSum <= rightSum)
        {
            result = Math.Max(result, leftSum + Best(left, mid));
        }

        if (rightSum <= leftSum)
        {
            result = Math.Max(result, rightSum + Best(mid + 1, right));
        }

        return result;
    }

    [Benchmark]
    public int MemoizedRecursion() => Memoizer.Memoize<(int Left, int Right), int>((0, PileCount - 1), BestMemoized);

    private int BestMemoized((int Left, int Right) range, Func<(int, int), int> best)
    {
        var (left, right) = range;

        if (left == right)
        {
            return 0;
        }

        var result = 0;

        for (var mid = left; mid < right; mid++)
        {
            result = BestForSplitMemoized(range, mid, result, best);
        }

        return result;
    }

    private int BestForSplitMemoized((int Left, int Right) range, int mid, int result, Func<(int, int), int> best)
    {
        var (left, right) = range;
        var leftSum = _prefix[mid + 1] - _prefix[left];
        var rightSum = _prefix[right + 1] - _prefix[mid + 1];

        if (leftSum <= rightSum)
        {
            result = Math.Max(result, leftSum + best((left, mid)));
        }

        if (rightSum <= leftSum)
        {
            result = Math.Max(result, rightSum + best((mid + 1, right)));
        }

        return result;
    }
}
