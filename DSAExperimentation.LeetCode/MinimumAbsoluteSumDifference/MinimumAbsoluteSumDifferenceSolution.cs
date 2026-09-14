using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.MinimumAbsoluteSumDifference;

// LeetCode 1818. Minimum Absolute Sum Difference: report the smallest achievable
// sum of |nums1[i] - nums2[i]|, modulo 1e9+7, after replacing at most one element of
// nums1 with any other element of nums1.
//
// One replacement can only shrink one term, so the answer is the untouched sum minus
// the single largest shrink available: for each index, how much closer to nums2[i]
// the nearest value anywhere in nums1 is than nums1[i] already was.
//
// The two strategies differ only in how they find that nearest value. The baseline
// rescans all of nums1 for every index; the composed strategy sorts one copy of
// nums1 with this repo's own MergeSort over ArrayIndexedSequence and then probes it
// per index with BinarySearch.LowerBound, whose insertion point brackets the closest
// value between the two entries on either side of it - O(n log n) rather than O(n^2).
//
// The subtraction happens before the modulo reduction on purpose: the true sum fits
// in a long, so no term is ever reduced past the point where the largest shrink is
// still the right one to subtract.
internal static class MinimumAbsoluteSumDifferenceSolution
{
    // The textbook answer: for every index, walk the whole of nums1 looking for the
    // closest replacement. BCL only - the arm the sorted strategy below has to
    // justify itself against.
    public static int MinAbsoluteSumDiffByFullRescan(int[] nums1, int[] nums2)
    {
        long baseSum = 0;
        long maxReduction = 0;

        for (var i = 0; i < nums1.Length; i++)
        {
            var diff = Math.Abs(nums1[i] - nums2[i]);
            baseSum += diff;

            var bestDiff = diff;

            for (var j = 0; j < nums1.Length; j++)
            {
                bestDiff = Math.Min(bestDiff, Math.Abs(nums1[j] - nums2[i]));
            }

            maxReduction = Math.Max(maxReduction, diff - bestDiff);
        }

        return (int)((baseSum - maxReduction) % ModularArithmetic.Modulo);
    }

    // The same answer, with the per-index rescan replaced by one sort plus a
    // lower-bound probe per index.
    public static int MinAbsoluteSumDiffBySortedBinarySearch(int[] nums1, int[] nums2)
    {
        var sorted = nums1.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));
        var sortedSequence = new ArraySequence<int>(sorted);

        long baseSum = 0;
        long maxReduction = 0;

        for (var i = 0; i < nums1.Length; i++)
        {
            var diff = Math.Abs(nums1[i] - nums2[i]);
            baseSum += diff;
            maxReduction = Math.Max(maxReduction, diff - ClosestDiff(sortedSequence, nums2[i], diff));
        }

        return (int)((baseSum - maxReduction) % ModularArithmetic.Modulo);
    }

    // How small one index's term can be made: LowerBound returns where target would
    // be inserted into the sorted copy, so the value nearest target is either the
    // entry at that insertion point or the one immediately before it. currentDiff is
    // the floor - leaving nums1[i] alone is always an option.
    private static int ClosestDiff(ArraySequence<int> sorted, int target, int currentDiff)
    {
        var insertion = BinarySearch.LowerBound<int, ArraySequence<int>>(sorted, target);
        var bestDiff = currentDiff;

        if (insertion < sorted.Length)
        {
            bestDiff = Math.Min(bestDiff, Math.Abs(sorted.Get(insertion) - target));
        }

        if (insertion > 0)
        {
            bestDiff = Math.Min(bestDiff, Math.Abs(sorted.Get(insertion - 1) - target));
        }

        return bestDiff;
    }
}
