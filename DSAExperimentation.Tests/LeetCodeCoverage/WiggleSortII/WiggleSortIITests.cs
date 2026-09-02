using DSAExperimentation.LeetCode.WiggleSortII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WiggleSortII;

// Harness only: both strategies live in WiggleSortIISolution and are asserted
// against the same examples - a value permutation check plus the strict wiggle
// property, since LC accepts any arrangement of the input's own values.
public sealed class WiggleSortIITests
{
    public static TheoryData<int[]> Examples =>
        new()
        {
            new[] { 1, 5, 1, 1, 6, 4 },
            new[] { 1, 3, 2, 2, 3, 1 },
            new[] { 4, 5, 5, 6 },
            new[] { 1, 1, 2, 1, 2, 2, 1 },
            new[] { 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void WiggleSortBySelectionSort_LeetCodeExamples_ProducesValidWigglePermutation(int[] nums)
    {
        var original = (int[])nums.Clone();
        var working = (int[])nums.Clone();

        WiggleSortIISolution.WiggleSortBySelectionSort(working);

        Assert.Equal(original.OrderBy(x => x), working.OrderBy(x => x));
        AssertWiggleProperty(working);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void WiggleSortByMergeSort_LeetCodeExamples_ProducesValidWigglePermutation(int[] nums)
    {
        var original = (int[])nums.Clone();
        var working = (int[])nums.Clone();

        WiggleSortIISolution.WiggleSortByMergeSort(working);

        Assert.Equal(original.OrderBy(x => x), working.OrderBy(x => x));
        AssertWiggleProperty(working);
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
