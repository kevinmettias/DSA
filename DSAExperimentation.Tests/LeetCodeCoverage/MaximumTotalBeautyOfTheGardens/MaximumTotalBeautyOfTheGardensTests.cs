using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumTotalBeautyOfTheGardens;

// LeetCode 2234. Maximum Total Beauty of the Gardens: sort ascending (MergeSort),
// then for every split point i, complete the top (n-i) gardens - sorted ascending,
// so those are always the cheapest (n-i) gardens to complete - and spend the
// remaining budget raising the bottom i gardens' minimum as high as possible,
// capped at target-1 (reaching target would make them complete instead, a case
// already covered by a different split). "How many of the bottom i gardens are
// below a candidate height" is a BinarySearch.LowerBound count over the sorted
// array - the same counting technique ClosestSubsequenceSumTests already uses.
// "The highest feasible height" is itself a monotone true/false search over an
// implicit sequence, resolved via BinarySearch.LowerBound the same way
// KokoEatingBananasTests resolves its own search-on-the-answer.
public sealed partial class MaximumTotalBeautyOfTheGardensTests
{
    [Fact]
    public void MaximumBeauty_FirstLeetCodeExample_ReturnsMaxAchievableBeauty()
    {
        int[] flowers = [1, 3, 1, 1];

        var actual = MaximumBeauty(flowers, newFlowers: 7, target: 6, new BeautyWeights(Full: 12, Partial: 1));

        Assert.Equal(14, actual);
    }

    [Fact]
    public void MaximumBeauty_SecondLeetCodeExample_ReturnsMaxAchievableBeauty()
    {
        int[] flowers = [2, 4, 5, 3];

        var actual = MaximumBeauty(flowers, newFlowers: 10, target: 5, new BeautyWeights(Full: 2, Partial: 6));

        Assert.Equal(30, actual);
    }

    [Fact]
    public void MaximumBeauty_EveryGardenAlreadyComplete_ScoresOnlyFullBeauty()
    {
        int[] flowers = [10, 10, 10];

        var actual = MaximumBeauty(flowers, newFlowers: 0, target: 5, new BeautyWeights(Full: 3, Partial: 100));

        Assert.Equal(9, actual);
    }

    private readonly record struct BeautyWeights(int Full, int Partial);

    private static long MaximumBeauty(int[] flowers, long newFlowers, int target, BeautyWeights weights)
    {
        var (n, prefixSum, suffixCost, sequence, anyGardenIncomplete) = PrepareSplitState(flowers, target);

        // Every nonempty prefix [0, i) includes index 0, the array's global minimum.
        // If that minimum is already >= target, no prefix can genuinely be
        // "incomplete" - the height search would otherwise walk all the way to the
        // target-1 cap with nothing actually costing anything (count-below-height
        // stays 0 the whole way) and report a fictitious partial-credit height for a
        // group that is, in reality, already fully complete. i=0 (all gardens forced
        // complete) already scores this case correctly, so every i>0 split just
        // withholds the partial term instead of fabricating one.
        var context = new SplitContext(n, newFlowers, anyGardenIncomplete, weights);
        var inputs = new HeightSearchInputs(sequence, prefixSum, target);

        return RunSplitSearch(n, suffixCost, context, inputs);
    }

    private static long RunSplitSearch(int n, long[] suffixCost, SplitContext context, HeightSearchInputs inputs)
    {
        var best = 0L;

        for (var i = 0; i <= n; i++)
        {
            var completionCost = suffixCost[i];

            if (completionCost <= context.Budget)
            {
                var beauty = BeautyForSplit(context, inputs, i, completionCost);
                best = Math.Max(best, beauty);
            }
        }

        return best;
    }

    private static (int N, long[] PrefixSum, long[] SuffixCost, ArraySequence<int> Sequence, bool AnyGardenIncomplete) PrepareSplitState(
        int[] flowers, int target)
    {
        var sorted = (int[])flowers.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var n = sorted.Length;
        var prefixSum = BuildPrefixSum(sorted);
        var suffixCost = BuildSuffixCompletionCost(sorted, target);
        var sequence = new ArraySequence<int>(sorted);
        var anyGardenIncomplete = sorted[0] < target;

        return (n, prefixSum, suffixCost, sequence, anyGardenIncomplete);
    }

    private readonly record struct SplitContext(int N, long Budget, bool AnyGardenIncomplete, BeautyWeights Weights);

    private readonly record struct HeightSearchInputs(ArraySequence<int> Sequence, long[] PrefixSum, int Target);

    private static long BeautyForSplit(SplitContext context, HeightSearchInputs inputs, int i, long completionCost)
    {
        var height = 0L;

        if (i > 0 && context.AnyGardenIncomplete)
        {
            var remaining = context.Budget - completionCost;
            height = MaxAchievableHeight(inputs.Sequence, new PrefixContext(inputs.PrefixSum, i), remaining, inputs.Target - 1);
        }

        return ((long)(context.N - i) * context.Weights.Full) + (height * context.Weights.Partial);
    }

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

    // The largest height every one of the bottom prefixCount gardens can be raised
    // to without exceeding remaining, found as the first infeasible height minus one
    // over an implicit feasibility sequence - height 0 is always feasible (costs
    // nothing), so the result is never negative.
    private static long MaxAchievableHeight(ArraySequence<int> sequence, PrefixContext prefix, long remaining, int capHeight)
    {
        var infeasibility = new HeightInfeasibleSequence(sequence, prefix, remaining, capHeight);
        return BinarySearch.LowerBound(infeasibility, true) - 1;
    }

    private static long CostToRaise(ArraySequence<int> sequence, PrefixContext prefix, int height)
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

        public bool Get(int index) => CostToRaise(sequence, prefix, index) > remaining;
    }
}
