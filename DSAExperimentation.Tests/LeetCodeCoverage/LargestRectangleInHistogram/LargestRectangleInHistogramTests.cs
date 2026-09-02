using DSAExperimentation.LeetCode.LargestRectangleInHistogram;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LargestRectangleInHistogram;

// Harness only. Both strategies live in LargestRectangleInHistogramSolution -
// this file just pins them to LeetCode's published examples plus a
// strictly-increasing run, where the best rectangle is a suffix rather than
// a single bar.
public sealed class LargestRectangleInHistogramTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 1, 5, 6, 2, 3], 10 },
            { [2, 4], 4 },
            { [1, 2, 3, 4, 5], 9 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestRectangleAreaByBruteForce_LeetCodeExamples_ReturnsMaxArea(
        int[] heights, int expected) =>
        Assert.Equal(expected, LargestRectangleInHistogramSolution.LargestRectangleAreaByBruteForce(heights));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestRectangleAreaByMonotonicStack_LeetCodeExamples_ReturnsMaxArea(
        int[] heights, int expected) =>
        Assert.Equal(expected, LargestRectangleInHistogramSolution.LargestRectangleAreaByMonotonicStack(heights));
}
