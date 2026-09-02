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

    // Halves int.MaxValue for a "no group found yet" sentinel that still tolerates
    // adding a real cost without overflowing.
    private const int InfinitySentinelDivisor = 2;

    // LC problem number, reused as the fixed benchmark-data seed.
    private const int RandomSeed = 1681;

    [Params(10, 14)]
    public int Length;

    private int[] _nums = null!;
    private int _fullMask;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
        _fullMask = (1 << Length) - 1;
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => Best(_fullMask);

    private int Best(int remaining) => ComputeBestCost(remaining, Best);

    [Benchmark]
    public int MemoizedRecursion() => Memoizer.Memoize<int, int>(_fullMask, BestMemoized);

    private int BestMemoized(int remaining, Func<int, int> best) => ComputeBestCost(remaining, best);

    // Shared recurrence body for both the unmemoized and memoized variants: they
    // differ only in how the "cost of the rest" is looked up, so that single point
    // of variation is passed in as `best`.
    private int ComputeBestCost(int remaining, Func<int, int> best)
    {
        if (remaining == 0)
        {
            return 0;
        }

        var lowestBit = remaining & -remaining;
        var others = remaining & ~lowestBit;

        return MinimizeOverSubsets(remaining, lowestBit, others, best);
    }

    private int MinimizeOverSubsets(int remaining, int lowestBit, int others, Func<int, int> best)
    {
        var result = int.MaxValue / InfinitySentinelDivisor;

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

            if (!TryIncludeMember(i, seen, ref min, ref max))
            {
                cost = 0;
                return false;
            }
        }

        cost = max - min;
        return true;
    }

    private bool TryIncludeMember(int index, Set<int> seen, ref int min, ref int max)
    {
        if (!seen.TryAdd(_nums[index]))
        {
            return false;
        }

        min = Math.Min(min, _nums[index]);
        max = Math.Max(max, _nums[index]);
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
