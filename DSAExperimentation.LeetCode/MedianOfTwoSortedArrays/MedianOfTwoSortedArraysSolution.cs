using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.LeetCode.MedianOfTwoSortedArrays;

// LeetCode 4. Median of Two Sorted Arrays: the median of the merged, sorted union of
// two already-sorted arrays, in better than O((m+n) log(m+n)).
//
// The two strategies differ only in whether they notice each array already arrived
// sorted. The baseline throws that away and re-sorts from scratch; the composed
// strategy binary-searches the smaller array for the correct partition using this
// repo's own generic BinarySearch.LowerBound, reused unmodified against
// PartitionFeasibilitySequence - a monotonic 0..0,1..1 witness over the partition
// index, exactly the "sorted ascending" shape LowerBound already assumes.
internal static class MedianOfTwoSortedArraysSolution
{
    private const int ParityModulus = 2;
    private const double AverageDivisor = 2.0;

    // The textbook fallback: concatenate both arrays and sort from scratch,
    // deliberately ignoring that each half already arrived sorted. Written without
    // this repo's primitives - it is the arm the composed solution below has to
    // justify itself against.
    public static double FindMedianByMergeAndSort(int[] nums1, int[] nums2)
    {
        var merged = nums1.Concat(nums2).OrderBy(value => value).ToArray();
        var mid = merged.Length / AlgorithmConstants.HalvingFactor;
        var hasOddLength = merged.Length % ParityModulus == 1;

        return hasOddLength
            ? ElementAt(merged, mid)
            : AverageOfMiddleTwo(merged, mid);
    }

    private static double AverageOfMiddleTwo(int[] values, int mid)
        => (values[mid - 1] + values[mid]) / AverageDivisor;

    public static double FindMedianByBinarySearchPartition(int[] nums1, int[] nums2)
    {
        if (nums1.Length > nums2.Length)
        {
            return FindMedianByBinarySearchPartition(nums2, nums1);
        }

        var m = nums1.Length;
        var n = nums2.Length;
        var half = (m + n + 1) / AlgorithmConstants.HalvingFactor;

        var (i, j) = FindPartitionIndices(nums1, nums2, half);
        var leftOfPartition = LeftOfPartition(nums1, nums2, i, j);

        if ((m + n) % ParityModulus == 1)
        {
            return leftOfPartition;
        }

        var rightOfPartition = RightOfPartition(nums1, nums2, i, j);

        return (leftOfPartition + rightOfPartition) / AverageDivisor;
    }

    private static (int I, int J) FindPartitionIndices(int[] nums1, int[] nums2, int half)
    {
        var feasibility = new PartitionFeasibilitySequence(nums1, nums2, half);
        var i = BinarySearch.LowerBound<int, PartitionFeasibilitySequence>(feasibility, 1) - 1;
        var j = half - i;

        return (i, j);
    }

    private static int LeftOfPartition(int[] nums1, int[] nums2, int i, int j)
        => Math.Max(
            i == 0 ? int.MinValue : ElementBefore(nums1, i),
            j == 0 ? int.MinValue : ElementBefore(nums2, j));

    private static int ElementBefore(int[] values, int index) => values[index - 1];

    private static int RightOfPartition(int[] nums1, int[] nums2, int i, int j)
        => Math.Min(
            i == nums1.Length ? int.MaxValue : ElementAt(nums1, i),
            j == nums2.Length ? int.MaxValue : ElementAt(nums2, j));

    private static int ElementAt(int[] values, int index) => values[index];
}
