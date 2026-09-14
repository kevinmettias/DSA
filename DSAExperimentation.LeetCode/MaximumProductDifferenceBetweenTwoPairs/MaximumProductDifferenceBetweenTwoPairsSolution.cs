using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximumProductDifferenceBetweenTwoPairs;

// LeetCode 1913. Maximum Product Difference Between Two Pairs: pick two
// index-disjoint pairs (w, x) and (y, z) maximizing (w * x) - (y * z).
//
// Every nums[i] is positive per LC's own constraint, which is what makes both
// strategies below safe without an explicit four-way-distinctness check: the
// array-wide largest product and the array-wide smallest product are always
// achieved by disjoint pairs once nums.Length >= 4, because the two largest
// elements and the two smallest are four different positions.
internal static class MaximumProductDifferenceBetweenTwoPairsSolution
{
    // Index offset of the second-largest element from the sorted array's end.
    private const int SecondFromEndOffset = 2;

    // The textbook O(n^2) answer: score every pair and keep the extremes. Pure
    // BCL - the arm the sorted strategy below has to justify itself against.
    public static int MaxProductDifferenceByBruteForcePairScan(int[] nums)
    {
        var maxProduct = int.MinValue;
        var minProduct = int.MaxValue;

        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                var product = nums[i] * nums[j];
                maxProduct = Math.Max(maxProduct, product);
                minProduct = Math.Min(minProduct, product);
            }
        }

        return maxProduct - minProduct;
    }

    // Sort with this repo's own MergeSort over ArrayIndexedSequence - the same
    // composition ArrayPartitionSolution uses for LC 561 - and read the answer
    // straight off the two ends, O(n log n) instead of O(n^2). The input array is
    // copied first because MergeSort sorts the sequence in place and a caller's
    // array is not the strategy's to reorder.
    public static int MaxProductDifferenceByMergeSortExtremes(int[] nums)
    {
        var sorted = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var n = sorted.Length;
        return (sorted[n - 1] * sorted[n - SecondFromEndOffset]) - (sorted[0] * sorted[1]);
    }
}
