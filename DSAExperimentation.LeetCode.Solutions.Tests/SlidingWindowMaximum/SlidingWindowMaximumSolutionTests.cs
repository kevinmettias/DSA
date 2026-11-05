using DSAExperimentation.LeetCode.Solutions.SlidingWindowMaximum;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SlidingWindowMaximum;

// Harness only. Both search strategies are SlidingWindowMaximumSolution's -
// this file just pins them to a window of size 1, where every element, negative or
// not, is its own maximum. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class SlidingWindowMaximumSolutionTests
{
    public static TheoryData<int[], int, int[]> Cases =>
        new()
        {
            { [4, -2, 9], 1, [4, -2, 9] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxSlidingWindowByBruteForceRescan_Cases_ReturnsPerWindowMaximums(
        int[] nums, int windowSize, int[] expected)
    {
        var actual = SlidingWindowMaximumSolution.MaxSlidingWindowByBruteForceRescan(nums, windowSize);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxSlidingWindowByMonotonicDeque_Cases_ReturnsPerWindowMaximums(
        int[] nums, int windowSize, int[] expected)
    {
        var actual = SlidingWindowMaximumSolution.MaxSlidingWindowByMonotonicDeque(nums, windowSize);

        Assert.Equal(expected, actual);
    }
}
