using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionArrayIntoTwoArraysToMinimizeSumDifference;

// LeetCode 2035. Partition Array Into Two Arrays to Minimize Sum Difference: the
// same meet-in-the-middle shape ClosestSubsequenceSumTests already uses -
// Backtrack.Search (the Subsets precedent) enumerates every subset sum of each
// n-element half, grouped by how many elements each subset took, MergeSort orders
// each group over an ArrayIndexedSequence, and BinarySearch.LowerBound locates the
// closest size-complementary partner sum on the sorted side - except here the two
// arrays must each end up with exactly n elements, so sums are grouped by subset
// size before searching instead of pooled together.
public sealed partial class PartitionArrayIntoTwoArraysToMinimizeSumDifferenceTests
{
    [Theory]
    [InlineData(new[] { 3, 9, 7, 3 }, 2)]
    [InlineData(new[] { -36, 36 }, 72)]
    [InlineData(new[] { 2, -1, 0, 4, -2, -9 }, 0)]
    public void MinimumDifference_LeetCodeExamples_ReturnsMinimumAchievableSumDifference(int[] nums, int expected)
        => Assert.Equal(expected, MinimumDifference(nums));

    private static int MinimumDifference(int[] nums)
    {
        var n = nums.Length / 2;
        var total = nums.Sum();
        var leftSumsByCount = SubsetSumsByCount(nums[..n], n);
        var rightSumsByCount = SubsetSumsByCount(nums[n..], n);
        var context = new SplitSearchContext(n, total, leftSumsByCount, rightSumsByCount);

        var best = int.MaxValue;

        for (var k = 0; k <= n; k++)
        {
            best = BestDifferenceForSplit(k, best, context);
        }

        return best;
    }

    // Every left-half subset of size k must pair with a right-half subset of
    // size (n - k) so both halves end up with exactly n elements; this finds
    // the closest-to-balanced pairing for that one split size.
    private static int BestDifferenceForSplit(int k, int best, SplitSearchContext context)
    {
        var rightSums = context.RightSumsByCount[context.N - k].ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(rightSums));
        var sequence = new ArraySequence<int>(rightSums);

        // rs closest to (total/2 - leftSum) minimizes |2*(leftSum+rs) - total|;
        // comparing 2*rs against the un-halved target avoids fractional search
        // keys entirely.
        var doubledComparer = Comparer<int>.Create((candidate, target) => (2 * candidate).CompareTo(target));
        var lookup = new RightSumLookup(rightSums, sequence, doubledComparer);

        foreach (var leftSum in context.LeftSumsByCount[k])
        {
            best = BestDifferenceForLeftSum(leftSum, context.Total, lookup, best);
        }

        return best;
    }

    private readonly record struct SplitSearchContext(int N, int Total, List<int>[] LeftSumsByCount, List<int>[] RightSumsByCount);

    private readonly record struct RightSumLookup(int[] RightSums, ArraySequence<int> Sequence, IComparer<int> DoubledComparer);

    // Of rightSums[index - 1] and rightSums[index] (the two right-half sums
    // straddling leftSum's ideal complement), keeps whichever pairing with
    // leftSum comes closest to a perfectly balanced split.
    private static int BestDifferenceForLeftSum(int leftSum, int total, RightSumLookup lookup, int best)
    {
        var target = total - (2 * leftSum);
        var index = BinarySearch.LowerBound<int, ArraySequence<int>>(lookup.Sequence, target, lookup.DoubledComparer);

        if (index < lookup.RightSums.Length)
        {
            best = Math.Min(best, Math.Abs((2 * (leftSum + lookup.RightSums[index])) - total));
        }

        if (index > 0)
        {
            best = Math.Min(best, Math.Abs((2 * (leftSum + lookup.RightSums[index - 1])) - total));
        }

        return best;
    }

    private static List<int>[] SubsetSumsByCount(int[] part, int n)
    {
        var sumsByCount = CreateEmptySumsByCount(n);
        CollectSubsetSums(part, sumsByCount);
        return sumsByCount;
    }

    private static List<int>[] CreateEmptySumsByCount(int n)
    {
        var sumsByCount = new List<int>[n + 1];
        for (var i = 0; i <= n; i++)
        {
            sumsByCount[i] = [];
        }

        return sumsByCount;
    }

    private static void CollectSubsetSums(int[] part, List<int>[] sumsByCount)
    {
        var state = new SumState();

        Backtrack.Search<SumState, int>(
            state,
            isSolution: s => s.NextIndex == part.Length,
            candidates: s => s.NextIndex < part.Length ? new[] { 0, 1 } : Array.Empty<int>(),
            choose: (s, take) =>
            {
                if (take == 1)
                {
                    s.Sum += part[s.NextIndex];
                    s.Count++;
                }

                s.NextIndex++;
            },
            unchoose: (s, take) =>
            {
                s.NextIndex--;

                if (take == 1)
                {
                    s.Sum -= part[s.NextIndex];
                    s.Count--;
                }
            },
            onSolution: s => sumsByCount[s.Count].Add(s.Sum));
    }

    private sealed class SumState
    {
        public int Sum { get; set; }
        public int Count { get; set; }
        public int NextIndex { get; set; }
    }
}
