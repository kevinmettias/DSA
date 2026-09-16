using DSAExperimentation.LeetCode.CountNonDecreasingSubarraysAfterKOperations;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountNonDecreasingSubarraysAfterKOperations;

// Harness only. Both counting strategies are
// CountNonDecreasingSubarraysAfterKOperationsSolution's - this file just pins them
// to LeetCode's published examples.
public sealed partial class CountNonDecreasingSubarraysAfterKOperationsTests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [6, 3, 1, 2, 4, 4], 7, 17 },
            { [6, 3, 1, 3, 6], 4, 12 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByPrefixMaxBruteForce_LeetCodeExamples_ReturnsCountOfFixableSubarrays(
        int[] nums, int maxOperations, long expected)
    {
        var actual = CountNonDecreasingSubarraysAfterKOperationsSolution.CountByPrefixMaxBruteForce(nums, maxOperations);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByMonotonicDequeWindow_LeetCodeExamples_ReturnsCountOfFixableSubarrays(
        int[] nums, int maxOperations, long expected)
    {
        var actual = CountNonDecreasingSubarraysAfterKOperationsSolution.CountByMonotonicDequeWindow(nums, maxOperations);
        Assert.Equal(expected, actual);
    }
}
