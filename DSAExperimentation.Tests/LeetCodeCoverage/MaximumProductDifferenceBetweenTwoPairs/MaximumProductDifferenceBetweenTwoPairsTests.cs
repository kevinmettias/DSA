using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumProductDifferenceBetweenTwoPairs;

// LeetCode 1913. Maximum Product Difference Between Two Pairs: sort with this
// repo's own MergeSort over ArrayIndexedSequence (same composition
// ArrayPartitionTests already uses) - the maximum product difference always pairs
// the two largest elements against the two smallest once sorted (every nums[i] is
// positive per LC's own constraint, so those two pairs are automatically
// index-disjoint whenever nums.Length >= 4), so no pair enumeration is needed.
public sealed partial class MaximumProductDifferenceBetweenTwoPairsTests
{
    [Fact]
    public void MaxProductDifference_ClassicExample_ReturnsMaximizedDifference()
    {
        int[] nums = [5, 6, 2, 7, 4];

        Assert.Equal(34, MaxProductDifference(nums));
    }

    [Fact]
    public void MaxProductDifference_SevenElements_ReturnsMaximizedDifference()
    {
        int[] nums = [4, 2, 5, 9, 7, 4, 8];

        Assert.Equal(64, MaxProductDifference(nums));
    }

    private static int MaxProductDifference(int[] nums)
    {
        var sorted = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var n = sorted.Length;
        return (sorted[n - 1] * sorted[n - 2]) - (sorted[0] * sorted[1]);
    }
}
