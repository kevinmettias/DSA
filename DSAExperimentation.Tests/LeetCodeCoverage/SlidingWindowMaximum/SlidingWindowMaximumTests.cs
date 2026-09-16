using DSAExperimentation.LeetCode.SlidingWindowMaximum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SlidingWindowMaximum;

// Harness only. Both search strategies are SlidingWindowMaximumSolution's -
// this file just pins them to LeetCode's published examples.
public sealed class SlidingWindowMaximumTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 3, -1, -3, 5, 3, 6, 7], 3, [3, 3, 5, 5, 6, 7] },
            { [4, -2, 9], 1, [4, -2, 9] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSlidingWindowByBruteForceRescan_LeetCodeExamples_ReturnsPerWindowMaximums(
        int[] nums, int windowSize, int[] expected)
    {
        var actual = SlidingWindowMaximumSolution.MaxSlidingWindowByBruteForceRescan(nums, windowSize);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSlidingWindowByMonotonicDeque_LeetCodeExamples_ReturnsPerWindowMaximums(
        int[] nums, int windowSize, int[] expected)
    {
        var actual = SlidingWindowMaximumSolution.MaxSlidingWindowByMonotonicDeque(nums, windowSize);

        Assert.Equal(expected, actual);
    }
}
