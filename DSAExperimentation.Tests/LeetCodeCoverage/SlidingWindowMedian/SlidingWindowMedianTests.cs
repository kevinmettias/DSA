using DSAExperimentation.LeetCode.SlidingWindowMedian;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SlidingWindowMedian;

// Harness only: both strategies live in SlidingWindowMedianSolution.
public sealed partial class SlidingWindowMedianTests
{
    public static TheoryData<int[], int, double[]> Examples =>
        new()
        {
            { [1, 3, -1, -3, 5, 3, 6, 7], 3, [1.0, -1.0, -1.0, 3.0, 5.0, 6.0] },
            { [1, 2, 3, 4, 2, 3, 1, 4, 2], 4, [2.5, 2.5, 3.0, 2.5, 2.5, 2.5] },
            { [4, -2, 9], 1, [4.0, -2.0, 9.0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MedianSlidingWindowBySortEachWindow_LeetCodeExamples_ReturnsPerWindowMedians(
        int[] nums, int windowSize, double[] expected)
    {
        var actual = SlidingWindowMedianSolution.MedianSlidingWindowBySortEachWindow(nums, windowSize);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MedianSlidingWindowByTwoHeapsLazyDeletion_LeetCodeExamples_ReturnsPerWindowMedians(
        int[] nums, int windowSize, double[] expected)
    {
        var actual = SlidingWindowMedianSolution.MedianSlidingWindowByTwoHeapsLazyDeletion(nums, windowSize);

        Assert.Equal(expected, actual);
    }
}
