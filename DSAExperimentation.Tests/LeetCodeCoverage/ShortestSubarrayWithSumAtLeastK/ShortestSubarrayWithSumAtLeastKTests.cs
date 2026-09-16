using DSAExperimentation.LeetCode.ShortestSubarrayWithSumAtLeastK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestSubarrayWithSumAtLeastK;

// Harness only. Both strategies are ShortestSubarrayWithSumAtLeastKSolution's - the
// O(n^2) prefix-pair scan and this repo's own Deque<int> monotonic prefix window -
// and this file pins both to LeetCode's published examples plus the negative-value
// cases a plain sliding window would get wrong.
public sealed partial class ShortestSubarrayWithSumAtLeastKTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1], 1, 1 },
            { [1, 2], 4, -1 },
            { [2, -1, 2], 3, 3 },
            { [2, -1, 2, -1, 2], 3, 3 },
            { [84, -37, 32, 40, 95], 167, 3 },
            { [17, 85, 93, -45, -21], 150, 2 },
            { [1, 2, 3, 4, 5], 15, 5 },
            { [-1, -2, -3], 1, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestSubarrayByBruteForcePrefixScan_LeetCodeExamples_ReturnsShortestQualifyingLength(
        int[] nums, int targetSum, int expected)
    {
        var actual = ShortestSubarrayWithSumAtLeastKSolution.ShortestSubarrayByBruteForcePrefixScan(nums, targetSum);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestSubarrayByMonotonicDeque_LeetCodeExamples_ReturnsShortestQualifyingLength(
        int[] nums, int targetSum, int expected)
    {
        var actual = ShortestSubarrayWithSumAtLeastKSolution.ShortestSubarrayByMonotonicDeque(nums, targetSum);

        Assert.Equal(expected, actual);
    }
}
