using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Incompatibility (LC 1681): the same canonical "always group the lowest
// still-ungrouped index" bitmask recursion, un-memoized (UnmemoizedRecursion,
// StoneGameVBenchmarks' own baseline shape) vs. cached per remaining-mask state by
// this repo's own Memoizer<TState,TResult> (MemoizedRecursion) - a perfect-
// matching-style search where the same "remaining elements" mask is reachable
// through more than one grouping order, so caching it once pays off over
// re-deriving it from scratch down every path. This repo's own Set<int> flags "two
// equal values in one subset" while a candidate group is built, in both variants.
[MemoryDiagnoser]
public class MinimumIncompatibilityBenchmarks
{
    private const int GroupSize = 2;

    [Params(10, 14)]
    public int Length;

    private int[] _nums = null!;
    private int _fullMask;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1681);
        _nums = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
        _fullMask = (1 << Length) - 1;
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => Best(_fullMask);

    private int Best(int remaining)
    {
        if (remaining == 0)
        {
            return 0;
        }

        var lowestBit = remaining & -remaining;
        var others = remaining & ~lowestBit;
        var best = int.MaxValue / 2;

        for (var sub = others; ; sub = (sub - 1) & others)
        {
            if (PopCount(sub) == GroupSize - 1 && TryGroupCost(sub | lowestBit, out var cost))
            {
                best = Math.Min(best, cost + Best(remaining & ~(sub | lowestBit)));
            }

            if (sub == 0)
            {
                break;
            }
        }

        return best;
    }

    [Benchmark]
    public int MemoizedRecursion() => Memoizer.Memoize<int, int>(_fullMask, BestMemoized);

    private int BestMemoized(int remaining, Func<int, int> best)
    {
        if (remaining == 0)
        {
            return 0;
        }

        var lowestBit = remaining & -remaining;
        var others = remaining & ~lowestBit;
        var result = int.MaxValue / 2;

        for (var sub = others; ; sub = (sub - 1) & others)
        {
            if (PopCount(sub) == GroupSize - 1 && TryGroupCost(sub | lowestBit, out var cost))
            {
                result = Math.Min(result, cost + best(remaining & ~(sub | lowestBit)));
            }

            if (sub == 0)
            {
                break;
            }
        }

        return result;
    }

    private bool TryGroupCost(int subset, out int cost)
    {
        var seen = new Set<int>();
        var min = int.MaxValue;
        var max = int.MinValue;

        for (var i = 0; i < _nums.Length; i++)
        {
            if ((subset & (1 << i)) == 0)
            {
                continue;
            }

            if (!seen.TryAdd(_nums[i]))
            {
                cost = 0;
                return false;
            }

            min = Math.Min(min, _nums[i]);
            max = Math.Max(max, _nums[i]);
        }

        cost = max - min;
        return true;
    }

    private static int PopCount(int value)
    {
        var count = 0;

        while (value != 0)
        {
            value &= value - 1;
            count++;
        }

        return count;
    }
}
