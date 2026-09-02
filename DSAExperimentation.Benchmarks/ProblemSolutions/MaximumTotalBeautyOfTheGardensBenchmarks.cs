using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Total Beauty of the Gardens (LC 2234): both strategies enumerate the same
// n+1 "complete vs. incomplete" splits over the sorted array, but differ in how they
// answer "how many of the incomplete prefix are below height h" and "what is the
// tallest affordable h" - a linear scan of the prefix and a linear walk down from the
// cap (no repo primitive) vs. this repo's own MergeSort (to sort) and
// BinarySearch.LowerBound (both for the below-height count and, over an implicit
// feasibility sequence, for the tallest affordable height itself -
// MaximumTotalBeautyOfTheGardensTests precedent). Length drives both the number of
// splits and, since Target scales with Length too, the per-split scan cost - so the
// O(n^2 * target) vs. O(n log n * log target) gap widens with it instead of staying
// flat at a fixed target.
[MemoryDiagnoser]
public class MaximumTotalBeautyOfTheGardensBenchmarks
{
    private const int Full = 50;
    private const int Partial = 10;
    private const int FlowerHeightRangeMultiplier = 2;

    [Params(200, 2_000)]
    public int Length;

    private int _target;
    private int[] _flowers = null!;
    private long _newFlowers;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _target = Length;
        _flowers = Enumerable.Range(0, Length).Select(_ => random.Next(1, (_target * FlowerHeightRangeMultiplier) + 1)).ToArray();

        // Deliberately scarce, not generous: a budget on the order of target alone
        // (rather than enough to raise a whole large prefix near the cap) forces the
        // achievable height down near 0 for most feasible splits, so
        // LinearSearchOnAnswer's height descent actually walks most of [0, target)
        // instead of succeeding on its very first probe at the cap.
        _newFlowers = _target;
    }

    [Benchmark(Baseline = true)]
    public long LinearSearchOnAnswer()
    {
        var sorted = (int[])_flowers.Clone();
        Array.Sort(sorted);

        var (n, prefixSum, suffixCost, context) = PrepareSplitState(sorted);

        return RunSplitSearch(n, suffixCost, context,
            (i, remaining) => LinearMaxAchievableHeight(sorted, new PrefixContext(prefixSum, i), remaining, _target - 1));
    }

    [Benchmark]
    public long SortAndBinarySearch()
    {
        var sorted = (int[])_flowers.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var (n, prefixSum, suffixCost, context) = PrepareSplitState(sorted);
        var sequence = new ArraySequence<int>(sorted);

        return RunSplitSearch(n, suffixCost, context,
            (i, remaining) => BinarySearchMaxAchievableHeight(sequence, new PrefixContext(prefixSum, i), remaining, _target - 1));
    }

    private (int N, long[] PrefixSum, long[] SuffixCost, SplitContext Context) PrepareSplitState(int[] sorted)
    {
        var n = sorted.Length;
        var prefixSum = BuildPrefixSum(sorted);
        var suffixCost = BuildSuffixCompletionCost(sorted, _target);
        var anyGardenIncomplete = sorted[0] < _target;

        return (n, prefixSum, suffixCost, new SplitContext(n, _newFlowers, anyGardenIncomplete));
    }

    // Shared by both variants: every split is enumerated and scored identically,
    // only how (and how expensively) the achievable height gets computed differs.
    private static long RunSplitSearch(int n, long[] suffixCost, SplitContext context, Func<int, long, long> heightAt)
    {
        var best = 0L;

        for (var i = 0; i <= n; i++)
        {
            var completionCost = suffixCost[i];

            if (completionCost <= context.Budget)
            {
                var beauty = BeautyForSplit(context, i, completionCost, remaining => heightAt(i, remaining));
                best = Math.Max(best, beauty);
            }
        }

        return best;
    }

    private static long BeautyForSplit(SplitContext context, int i, long completionCost, Func<long, long> heightAt)
    {
        var height = 0L;

        if (i > 0 && context.AnyGardenIncomplete)
        {
            height = heightAt(context.Budget - completionCost);
        }

        return ((long)(context.N - i) * Full) + (height * Partial);
    }

    private readonly record struct SplitContext(int N, long Budget, bool AnyGardenIncomplete);

    private static long[] BuildPrefixSum(int[] sorted)
    {
        var prefixSum = new long[sorted.Length + 1];

        for (var i = 0; i < sorted.Length; i++)
        {
            prefixSum[i + 1] = prefixSum[i] + sorted[i];
        }

        return prefixSum;
    }

    private static long[] BuildSuffixCompletionCost(int[] sorted, int target)
    {
        var suffixCost = new long[sorted.Length + 1];

        for (var i = sorted.Length - 1; i >= 0; i--)
        {
            suffixCost[i] = suffixCost[i + 1] + Math.Max(target - sorted[i], 0);
        }

        return suffixCost;
    }

    // No repo primitive: counts below height via a linear prefix scan (with an early
    // break once sorted values reach h), then walks candidate heights down from the
    // cap one at a time until one is affordable.
    private static long LinearMaxAchievableHeight(int[] sorted, PrefixContext prefix, long remaining, int capHeight)
    {
        for (var height = capHeight; height > 0; height--)
        {
            if (LinearCostToRaise(sorted, prefix, height) <= remaining)
            {
                return height;
            }
        }

        return 0;
    }

    private static long LinearCostToRaise(int[] sorted, PrefixContext prefix, int height)
    {
        var count = 0;

        for (var j = 0; j < prefix.Count && sorted[j] < height; j++)
        {
            count++;
        }

        return ((long)height * count) - prefix.Sum[count];
    }

    private static long BinarySearchMaxAchievableHeight(ArraySequence<int> sequence, PrefixContext prefix, long remaining, int capHeight)
    {
        var infeasibility = new HeightInfeasibleSequence(sequence, prefix, remaining, capHeight);
        return BinarySearch.LowerBound(infeasibility, true) - 1;
    }

    private static long BinarySearchCostToRaise(ArraySequence<int> sequence, PrefixContext prefix, int height)
    {
        var indexAtHeight = BinarySearch.LowerBound<int, ArraySequence<int>>(sequence, height);
        var countBelow = Math.Min(indexAtHeight, prefix.Count);
        return ((long)height * countBelow) - prefix.Sum[countBelow];
    }

    // Bundles the whole-array prefix-sum table with a specific split index: every
    // height-cost computation needs both together, and they always travel as a pair.
    private readonly record struct PrefixContext(long[] Sum, int Count);

    private readonly struct HeightInfeasibleSequence(
        ArraySequence<int> sequence, PrefixContext prefix, long remaining, int capHeight)
        : IRandomAccessSequence<bool>
    {
        public int Length => capHeight + 1;

        public bool Get(int index) => BinarySearchCostToRaise(sequence, prefix, index) > remaining;
    }
}
