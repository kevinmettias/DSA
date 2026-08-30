using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ArrayPartition;

// LeetCode 561. Array Partition: sort with this repo's MergeSort over
// ArrayIndexedSequence (same composition ThreeSumTests already uses), then sum
// every even-indexed element of the sorted array - pairing each element with its
// immediate neighbor always maximizes the sum of pair-minimums once sorted.
public sealed partial class ArrayPartitionTests
{
    [Fact]
    public void ArrayPairSum_ClassicExample_ReturnsMaximizedMinPairSum()
    {
        int[] nums = [1, 4, 3, 2];

        Assert.Equal(4, ArrayPairSum(nums));
    }

    [Fact]
    public void ArrayPairSum_SixElements_ReturnsMaximizedMinPairSum()
    {
        int[] nums = [6, 2, 6, 5, 1, 2];

        Assert.Equal(9, ArrayPairSum(nums));
    }

    private static int ArrayPairSum(int[] nums)
    {
        var sorted = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var sum = 0;
        for (var i = 0; i < sorted.Length; i += 2)
        {
            sum += sorted[i];
        }

        return sum;
    }
}
