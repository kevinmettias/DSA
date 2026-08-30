using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WiggleSortII;

// LeetCode 324. Wiggle Sort II: sort with this repo's own MergeSort over
// ArrayIndexedSequence (HIndex precedent), then interleave the reversed lower and
// upper halves into the even/odd index positions. Filling from the reversed halves
// - rather than a naive ascending interleave - is what keeps the result valid even
// when the input has repeated values clustered around the median.
public sealed partial class WiggleSortIITests
{
    [Theory]
    [InlineData(new[] { 1, 5, 1, 1, 6, 4 })]
    [InlineData(new[] { 1, 3, 2, 2, 3, 1 })]
    [InlineData(new[] { 4, 5, 5, 6 })]
    [InlineData(new[] { 1, 1, 2, 1, 2, 2, 1 })]
    [InlineData(new[] { 1 })]
    public void WiggleSort_LeetCodeExamples_ProducesValidWigglePermutation(int[] nums)
    {
        var original = (int[])nums.Clone();

        WiggleSort(nums);

        Assert.Equal(original.OrderBy(x => x), nums.OrderBy(x => x));
        AssertWiggleProperty(nums);
    }

    private static void WiggleSort(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var n = nums.Length;
        var lowIndex = (n - 1) / 2;
        var highIndex = n - 1;

        for (var i = 0; i < n; i++)
        {
            nums[i] = i % 2 == 0 ? sorted[lowIndex--] : sorted[highIndex--];
        }
    }

    private static void AssertWiggleProperty(int[] nums)
    {
        for (var i = 0; i < nums.Length - 1; i++)
        {
            if (i % 2 == 0)
            {
                Assert.True(nums[i] < nums[i + 1], $"Expected nums[{i}] < nums[{i + 1}]");
            }
            else
            {
                Assert.True(nums[i] > nums[i + 1], $"Expected nums[{i}] > nums[{i + 1}]");
            }
        }
    }
}
