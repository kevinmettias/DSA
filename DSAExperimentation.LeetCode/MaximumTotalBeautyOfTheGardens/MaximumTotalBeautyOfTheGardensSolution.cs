using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximumTotalBeautyOfTheGardens;

// LeetCode 2234. Maximum Total Beauty of the Gardens: spend newFlowers across the
// gardens to maximise full * (complete gardens) + partial * (smallest garden's
// flower count).
//
// Both strategies enumerate the same n+1 splits of the ascending-sorted array: for
// a split at i, the top n-i gardens are completed (sorted ascending, so those are
// always the cheapest ones to complete) and whatever budget survives raises the
// bottom i gardens' minimum as high as it can, capped at target-1 - reaching target
// would make them complete instead, which is a different split's case.
//
// They differ only in how the two questions inside a split are answered:
//
//   - "how many of the bottom i gardens sit below a candidate height h" - a linear
//     scan of the prefix, or a BinarySearch.LowerBound count over the sorted array,
//     the same counting technique ClosestSubsequenceSum uses;
//   - "what is the tallest affordable h" - a walk down from the cap one height at a
//     time, or the leftmost infeasible height found by BinarySearch.LowerBound over
//     an implicit feasibility sequence, the same search-on-the-answer shape
//     KokoEatingBananas uses.
internal static class MaximumTotalBeautyOfTheGardensSolution
{
    // The textbook answer: BCL Array.Sort, a linear count of the prefix below a
    // candidate height, and a linear descent from the cap until a height is
    // affordable. Deliberately written without this repo's primitives - it is the
    // arm the composed strategy below has to justify itself against, and its
    // O(n * target) split cost is what the comparison is about.
    public static long MaximumBeautyByLinearSearchOnAnswer(
        int[] flowers, long newFlowers, int target, BeautyWeights weights)
    {
        var sorted = (int[])flowers.Clone();
        Array.Sort(sorted);

        var (context, prefixSum, suffixCost) =
            PrepareSplitState(sorted, newFlowers, target, weights);

        return RunSplitSearch(context, suffixCost, new MaxHeightByLinearScan(sorted, prefixSum, target - 1));
    }

    // No repo primitive: walks candidate heights down from the cap one at a time
    // until one is affordable, counting the prefix below each by a linear scan.
    // Height 0 always costs nothing, so the result is never negative.
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

    // This repo's own MergeSort over an ArrayIndexedSequence to order the gardens,
    // then BinarySearch.LowerBound twice per probe: once over the sorted heights to
    // count what sits below a candidate height, once over an implicit
    // IRandomAccessSequence<bool> of infeasibility to find the tallest height the
    // remaining budget can pay for.
    public static long MaximumBeautyBySortAndBinarySearch(
        int[] flowers, long newFlowers, int target, BeautyWeights weights)
    {
        var sorted = (int[])flowers.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var (context, prefixSum, suffixCost) =
            PrepareSplitState(sorted, newFlowers, target, weights);
        var sequence = new ArraySequence<int>(sorted);

        return RunSplitSearch(context, suffixCost, new MaxHeightByBinarySearch(sequence, prefixSum, target - 1));
    }

    // The largest height every one of the bottom prefix.Count gardens can be raised
    // to without exceeding remaining, found as the first infeasible height minus one
    // over the implicit feasibility sequence below - height 0 is always feasible
    // (it costs nothing), so the result is never negative.
    private static long BinarySearchMaxAchievableHeight(
        ArraySequence<int> sequence, PrefixContext prefix, long remaining, int capHeight)
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

    private static (SplitContext Context, long[] PrefixSum, long[] SuffixCost) PrepareSplitState(
        int[] sorted, long newFlowers, int target, BeautyWeights weights)
    {
        var prefixSum = BuildPrefixSum(sorted);
        var suffixCost = BuildSuffixCompletionCost(sorted, target);
        var context = new SplitContext(sorted.Length, newFlowers, sorted[0] < target, weights);

        return (context, prefixSum, suffixCost);
    }

    // The one question the two strategies answer with different code: for a prefix of
    // `prefixCount` gardens and a given remaining budget, the tallest height every one
    // of them can be raised to. The ordering, the prefix sums and the cap are all
    // settled before either strategy asks, so only what varies from split to split
    // travels through this interface.
    private interface IMaxAchievableHeight
    {
        long Compute(int prefixCount, long remaining);
    }

    // Shared by both strategies: every split is enumerated and scored identically,
    // only how (and how expensively) the achievable height gets computed differs.
    private static long RunSplitSearch(SplitContext context, long[] suffixCost, IMaxAchievableHeight heightAt)
    {
        var best = 0L;

        for (var i = 0; i <= context.N; i++)
        {
            var completionCost = suffixCost[i];

            if (completionCost <= context.Budget)
            {
                var beauty = BeautyForSplit(context, i, completionCost, heightAt);

                best = Math.Max(best, beauty);
            }
        }

        return best;
    }

    private static long BeautyForSplit(SplitContext context, int i, long completionCost, IMaxAchievableHeight heightAt)
    {
        var height = 0L;

        if (i > 0 && context.AnyGardenIncomplete)
        {
            height = heightAt.Compute(i, context.Budget - completionCost);
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

    // Everything a split's score depends on that does not change from split to
    // split. AnyGardenIncomplete records whether the global minimum starts below
    // target: every nonempty prefix [0, i) contains index 0, so if that minimum is
    // already complete no prefix can genuinely be incomplete, and the height search
    // would otherwise walk all the way to the target-1 cap with nothing actually
    // costing anything (the count-below-height stays 0 the whole way) and report a
    // fictitious partial-credit height for a group that is in reality already
    // complete. The i=0 split already scores that case correctly, so every i>0 split
    // withholds the partial term instead of fabricating one.
    private readonly record struct SplitContext(int N, long Budget, bool AnyGardenIncomplete, BeautyWeights Weights);

    // Bundles the whole-array prefix-sum table with a specific split index: every
    // height-cost computation needs both together, and they always travel as a pair.
    private readonly record struct PrefixContext(long[] Sum, int Count);

    // Get(index) is "raising the bottom prefix.Count gardens to height index costs
    // more than remaining" - false up to the tallest affordable height and true from
    // there on, the monotonicity BinarySearch.LowerBound assumes but never checks. A
    // witness for this problem alone: the cost rule is LC 2234's own content, not a
    // general monotone-predicate shape.
    private readonly struct HeightInfeasibleSequence(
        ArraySequence<int> sequence, PrefixContext prefix, long remaining, int capHeight)
        : IRandomAccessSequence<bool>
    {
        public int Length => capHeight + 1;

        public bool Get(int index) => BinarySearchCostToRaise(sequence, prefix, index) > remaining;
    }

    // The baseline arm's mechanism: walk candidate heights down from the cap one at a
    // time until one is affordable, counting the prefix below each by a linear scan.
    private sealed class MaxHeightByLinearScan(int[] sorted, long[] prefixSum, int capHeight) : IMaxAchievableHeight
    {
        public long Compute(int prefixCount, long remaining) =>
            LinearMaxAchievableHeight(sorted, new PrefixContext(prefixSum, prefixCount), remaining, capHeight);
    }

    // The composed arm's mechanism: the leftmost infeasible height over the implicit
    // feasibility sequence, found by BinarySearch.LowerBound.
    private sealed class MaxHeightByBinarySearch(ArraySequence<int> sequence, long[] prefixSum, int capHeight) : IMaxAchievableHeight
    {
        public long Compute(int prefixCount, long remaining) =>
            BinarySearchMaxAchievableHeight(sequence, new PrefixContext(prefixSum, prefixCount), remaining, capHeight);
    }
}
