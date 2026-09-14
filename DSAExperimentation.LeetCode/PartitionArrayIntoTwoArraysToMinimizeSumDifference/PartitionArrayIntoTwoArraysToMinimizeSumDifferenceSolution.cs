using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.PartitionArrayIntoTwoArraysToMinimizeSumDifference;

// LeetCode 2035. Partition Array Into Two Arrays to Minimize Sum Difference: split
// a 2n-element array into two arrays of exactly n elements each and minimize the
// absolute difference of their sums.
//
// A size-n subset with sum s leaves total - s behind, so the difference is
// |2s - total|; both strategies here are different ways of hunting the size-n
// subset sum closest to half the total.
//
// - MinimumDifferenceByBruteForceEqualSplits walks all 2^(2n) bitmasks over the
//   whole array and prices the ones whose popcount is n. Deliberately plain BCL -
//   this is the "what you would write without this repo" arm, and it was
//   previously the benchmark's unasserted baseline.
// - MinimumDifferenceByMeetInTheMiddle is the same shape ClosestSubsequenceSum
//   uses for LC 1755, with one extra constraint: because both output arrays must
//   end up with exactly n elements, subset sums are grouped by how many elements
//   they took before being searched, instead of pooled together. Backtrack.Search
//   (the Subsets precedent) enumerates each n-element half's 2^n subset sums,
//   MergeSort orders each size-group over an ArrayIndexedSequence, and
//   BinarySearch.LowerBound locates the closest size-complementary partner sum.
internal static class PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution
{
    // The input holds 2n elements and each output array takes n of them.
    private const int PartitionRatio = 2;

    // The difference between a subset summing to s and its complement is
    // |2s - total|, so every candidate is priced against the un-halved total.
    private const int SumDifferenceScale = 2;

    // The take/skip decision Backtrack.Search branches on at each position.
    private const int Taken = 1;

    public static int MinimumDifferenceByBruteForceEqualSplits(int[] nums)
    {
        var half = nums.Length / PartitionRatio;
        var total = nums.Sum();
        var best = int.MaxValue;

        for (var mask = 0; mask < (1 << nums.Length); mask++)
        {
            var splitDifference = EqualSplitDifference(nums, mask, half, total);
            best = Math.Min(best, splitDifference);
        }

        return best;
    }

    // A mask that does not take exactly half the elements is not a legal
    // partition, so it is priced out of contention rather than skipped.
    private static int EqualSplitDifference(int[] nums, int mask, int half, int total)
    {
        var count = 0;
        var sum = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                count++;
                sum += nums[i];
            }
        }

        return count == half ? Math.Abs((SumDifferenceScale * sum) - total) : int.MaxValue;
    }

    public static int MinimumDifferenceByMeetInTheMiddle(int[] nums)
    {
        var half = nums.Length / PartitionRatio;
        var total = nums.Sum();
        var context = new SplitSearchContext(
            half,
            total,
            SubsetSumsByCount(nums[..half], half),
            SubsetSumsByCount(nums[half..], half));

        var best = int.MaxValue;

        for (var taken = 0; taken <= half; taken++)
        {
            var splitDifference = BestDifferenceForSplit(taken, context);
            best = Math.Min(best, splitDifference);
        }

        return best;
    }

    // Every subset sum of one half, bucketed by how many elements the subset took,
    // as a take/skip decision per position: the state carries the running sum and
    // count, so Unchoose only has to undo what Choose added.
    private static List<int>[] SubsetSumsByCount(int[] part, int half)
    {
        var sumsByCount = CreateEmptyGroups(half);
        var state = new SumState();

        Backtrack.Search<SumState, int>(
            state,
            isSolution: s => s.NextIndex == part.Length,
            candidates: s => s.NextIndex < part.Length ? SkipOrTake() : Array.Empty<int>(),
            choose: (s, take) => ChooseSubsetElement(s, take, part),
            unchoose: (s, take) => UnchooseSubsetElement(s, take, part),
            onSolution: s => sumsByCount[s.Count].Add(s.Sum));

        return sumsByCount;
    }

    private static List<int>[] CreateEmptyGroups(int half)
    {
        var groups = new List<int>[half + 1];

        for (var i = 0; i <= half; i++)
        {
            groups[i] = [];
        }

        return groups;
    }

    // The two choices open at every position: leave the element out of the subset,
    // or take it in.
    private static int[] SkipOrTake() => [0, Taken];

    private static void ChooseSubsetElement(SumState state, int take, int[] part)
    {
        if (take == Taken)
        {
            state.Sum += part[state.NextIndex];
            state.Count++;
        }

        state.NextIndex++;
    }

    private static void UnchooseSubsetElement(SumState state, int take, int[] part)
    {
        state.NextIndex--;

        if (take == Taken)
        {
            state.Sum -= part[state.NextIndex];
            state.Count--;
        }
    }

    // Every left-half subset of size k must pair with a right-half subset of size
    // (n - k) so both output arrays end up with exactly n elements; this finds the
    // closest-to-balanced pairing for that one split size.
    private static int BestDifferenceForSplit(int taken, SplitSearchContext context)
    {
        var rightSums = context.RightSumsByCount[context.Half - taken].ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(rightSums));

        // The right sum closest to half the total minus the left sum minimizes the
        // difference; comparing 2*candidate against the un-halved target keeps the
        // search key integral instead of fractional.
        var doubledComparer = Comparer<int>.Create(
            (candidate, target) => (SumDifferenceScale * candidate).CompareTo(target));
        var lookup = new RightSumLookup(rightSums, new ArraySequence<int>(rightSums), doubledComparer);

        var best = int.MaxValue;

        foreach (var leftSum in context.LeftSumsByCount[taken])
        {
            var complementDifference = ClosestComplementDifference(leftSum, lookup, context.Total);
            best = Math.Min(best, complementDifference);
        }

        return best;
    }

    // Of rightSums[index - 1] and rightSums[index] - the two right-half sums
    // straddling the ideal complement of leftSum - keeps whichever pairing comes
    // closest to a perfectly balanced split.
    private static int ClosestComplementDifference(int leftSum, RightSumLookup lookup, int total)
    {
        var target = total - (SumDifferenceScale * leftSum);
        var index = BinarySearch.LowerBound<int, ArraySequence<int>>(
            lookup.Sequence, target, lookup.DoubledComparer);
        var best = int.MaxValue;

        if (index < lookup.Values.Length)
        {
            best = Math.Min(best, Math.Abs((SumDifferenceScale * (leftSum + lookup.Values[index])) - total));
        }

        if (index > 0)
        {
            best = Math.Min(best, Math.Abs((SumDifferenceScale * (leftSum + lookup.Values[index - 1])) - total));
        }

        return best;
    }

    private readonly record struct SplitSearchContext(
        int Half,
        int Total,
        List<int>[] LeftSumsByCount,
        List<int>[] RightSumsByCount);

    private readonly record struct RightSumLookup(
        int[] Values,
        ArraySequence<int> Sequence,
        IComparer<int> DoubledComparer);

    private sealed class SumState
    {
        public int Sum { get; set; }
        public int Count { get; set; }
        public int NextIndex { get; set; }
    }
}
