using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumAbsoluteSumDifference;

// LeetCode 1818. Minimum Absolute Sum Difference: sort a copy of nums1 with this
// repo's own MergeSort over ArrayIndexedSequence (the same composition
// ArrayPartitionTests/AssignCookiesTests already use), then for each index probe the
// sorted copy with BinarySearch.LowerBound to find nums1's closest replacement value
// to nums2[i] - the largest single-index swap can only ever reduce the sum by
// replacing exactly one term with its nearest available neighbor, so the answer is
// the base absolute-difference sum minus the single biggest such reduction.
public sealed partial class MinimumAbsoluteSumDifferenceTests
{
    [Fact]
    public void MinAbsoluteSumDiff_LeetCodeExampleOne_ReturnsThree()
    {
        int[] nums1 = [1, 7, 5];
        int[] nums2 = [2, 3, 5];

        var minAbsoluteSumDiff = MinAbsoluteSumDiff(nums1, nums2);
        Assert.Equal(3, minAbsoluteSumDiff);
    }

    [Fact]
    public void MinAbsoluteSumDiff_IdenticalArrays_ReturnsZero()
    {
        int[] nums1 = [2, 4, 6, 8, 10];
        int[] nums2 = [2, 4, 6, 8, 10];

        var minAbsoluteSumDiff = MinAbsoluteSumDiff(nums1, nums2);
        Assert.Equal(0, minAbsoluteSumDiff);
    }

    [Fact]
    public void MinAbsoluteSumDiff_LeetCodeExampleThree_ReturnsTwenty()
    {
        int[] nums1 = [1, 10, 4, 4, 2, 7];
        int[] nums2 = [9, 3, 5, 1, 7, 4];

        var minAbsoluteSumDiff = MinAbsoluteSumDiff(nums1, nums2);
        Assert.Equal(20, minAbsoluteSumDiff);
    }

    private static int MinAbsoluteSumDiff(int[] nums1, int[] nums2)
    {
        const int mod = 1_000_000_007;

        var sorted = nums1.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));
        var sortedSequence = new ArraySequence<int>(sorted);

        long baseSum = 0;
        long maxReduction = 0;

        for (var i = 0; i < nums1.Length; i++)
        {
            var (diff, reduction) = EvaluateReplacement(nums1[i], nums2[i], sorted, sortedSequence);
            baseSum += diff;
            maxReduction = Math.Max(maxReduction, reduction);
        }

        return (int)((baseSum - maxReduction) % mod);
    }

    // One index's contribution: its base |nums1[i]-nums2[i]| difference, and how
    // much that difference would shrink by if nums1[i] were replaced with its
    // nearest neighbor (by lower-bound insertion point) in the sorted copy.
    private static (int Diff, int Reduction) EvaluateReplacement(int value1, int value2, int[] sorted, ArraySequence<int> sortedSequence)
    {
        var diff = Math.Abs(value1 - value2);
        var insertion = BinarySearch.LowerBound<int, ArraySequence<int>>(sortedSequence, value2);
        var bestDiff = diff;

        if (insertion < sorted.Length)
        {
            bestDiff = Math.Min(bestDiff, Math.Abs(sorted[insertion] - value2));
        }

        if (insertion > 0)
        {
            bestDiff = Math.Min(bestDiff, Math.Abs(sorted[insertion - 1] - value2));
        }

        return (diff, diff - bestDiff);
    }
}
