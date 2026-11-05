using DSAExperimentation.LeetCode.Solutions.ShortestSubarrayWithSumAtLeastK;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ShortestSubarrayWithSumAtLeastK;

// Harness only. Both strategies are ShortestSubarrayWithSumAtLeastKSolution's - the
// O(n^2) prefix-pair scan and this repo's MonotonicDeque over the prefix sums -
// and this file pins both to the negative-value
// cases a plain sliding window would get wrong.
public sealed partial class ShortestSubarrayWithSumAtLeastKSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [2, -1, 2, -1, 2], 3, 3 },
            { [17, 85, 93, -45, -21], 150, 2 },
            { [-1, -2, -3], 1, -1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ShortestSubarrayByBruteForcePrefixScan_Cases_ReturnsShortestQualifyingLength(
        int[] nums, int targetSum, int expected)
    {
        var actual = ShortestSubarrayWithSumAtLeastKSolution.ShortestSubarrayByBruteForcePrefixScan(nums, targetSum);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void ShortestSubarrayByMonotonicDeque_Cases_ReturnsShortestQualifyingLength(
        int[] nums, int targetSum, int expected)
    {
        var actual = ShortestSubarrayWithSumAtLeastKSolution.ShortestSubarrayByMonotonicDeque(nums, targetSum);

        Assert.Equal(expected, actual);
    }
}
