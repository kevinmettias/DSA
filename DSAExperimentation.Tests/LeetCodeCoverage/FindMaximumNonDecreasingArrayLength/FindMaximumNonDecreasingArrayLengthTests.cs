using DSAExperimentation.LeetCode.FindMaximumNonDecreasingArrayLength;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindMaximumNonDecreasingArrayLength;

// Harness only: both strategies live in
// FindMaximumNonDecreasingArrayLengthSolution and are asserted against the same
// examples, including an already non-decreasing array (no merges needed at all)
// and one where every element must merge into a single block.
public sealed class FindMaximumNonDecreasingArrayLengthTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [5, 2, 2], 1 },
            { [1, 2, 3, 4], 4 },
            { [4, 3, 2, 6], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMaxLengthByBruteForceDp_LeetCodeExamples_ReturnsMaximumLength(int[] nums, int expected) =>
        Assert.Equal(expected, FindMaximumNonDecreasingArrayLengthSolution.FindMaxLengthByBruteForceDp(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMaxLengthByMonotonicStackBinarySearch_LeetCodeExamples_ReturnsMaximumLength(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            FindMaximumNonDecreasingArrayLengthSolution.FindMaxLengthByMonotonicStackBinarySearch(nums));
}
