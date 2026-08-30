using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HowManyNumbersAreSmallerThanTheCurrentNumber;

// LeetCode 1365. How Many Numbers Are Smaller Than the Current Number: sort a
// copy with this repo's own MergeSort, then for each original value look up
// BinarySearch.LowerBound in that sorted copy - the leftmost insertion index
// LowerBound already computes is exactly the count of strictly smaller elements.
public sealed partial class HowManyNumbersAreSmallerThanTheCurrentNumberTests
{
    [Fact]
    public void SmallerNumbersThanCurrent_ClassicExample_ReturnsCountsPerElement()
    {
        int[] nums = [8, 1, 2, 2, 3];

        var result = SmallerNumbersThanCurrent(nums);

        Assert.Equal([4, 0, 1, 1, 3], result);
    }

    [Fact]
    public void SmallerNumbersThanCurrent_AllEqualValues_ReturnsAllZeros()
    {
        int[] nums = [7, 7, 7, 7];

        var result = SmallerNumbersThanCurrent(nums);

        Assert.Equal([0, 0, 0, 0], result);
    }

    private static int[] SmallerNumbersThanCurrent(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var sequence = new ArraySequence<int>(sorted);
        var result = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            result[i] = BinarySearch.LowerBound<int, ArraySequence<int>>(sequence, nums[i]);
        }

        return result;
    }
}
