using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Stone Game VII (LC 1690): plain un-memoized minimax recursion over
// (left, right) bounds - exponential, since the same (left, right) sub-range
// recurs through many different removal orders - vs. this repo's own
// Memoizer<TState,TResult> caching that exact pair, the identical shape
// StoneGameBenchmarks already uses for LC 877's own (left,right) minimax
// recurrence. PileCount is kept modest for the same reason StoneGameBenchmarks
// documents: the un-memoized baseline's blowup is real.
[MemoryDiagnoser]
public class StoneGameVIIBenchmarks
{
    [Params(22, 26)]
    public int PileCount;

    private int[] _prefix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1690);
        var stones = Enumerable.Range(0, PileCount).Select(_ => random.Next(1, 100)).ToArray();
        _prefix = new int[PileCount + 1];

        for (var i = 0; i < PileCount; i++)
        {
            _prefix[i + 1] = _prefix[i] + stones[i];
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

        var removeLeftScore = _prefix[right + 1] - _prefix[left + 1];
        var removeRightScore = _prefix[right] - _prefix[left];

        var takeLeft = removeLeftScore - Best(left + 1, right);
        var takeRight = removeRightScore - Best(left, right - 1);
        return Math.Max(takeLeft, takeRight);
    }

    [Benchmark]
    public int MemoizedRecursion()
        => Memoizer.Memoize<(int Left, int Right), int>((0, PileCount - 1), BestMemoized);

    private int BestMemoized((int Left, int Right) range, Func<(int Left, int Right), int> bestDiff)
    {
        var (left, right) = range;
        if (left == right)
        {
            return 0;
        }

        var removeLeftScore = _prefix[right + 1] - _prefix[left + 1];
        var removeRightScore = _prefix[right] - _prefix[left];

        var takeLeft = removeLeftScore - bestDiff((left + 1, right));
        var takeRight = removeRightScore - bestDiff((left, right - 1));
        return Math.Max(takeLeft, takeRight);
    }
}
