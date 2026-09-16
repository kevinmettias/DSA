using DSAExperimentation.LeetCode.MaximumLengthOfRepeatedSubarray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumLengthOfRepeatedSubarray;

// Harness only. Both strategies are MaximumLengthOfRepeatedSubarraySolution's -
// FindLengthByBruteForce (previously untested scaffolding inlined in the benchmark)
// gets the same LeetCode examples as FindLengthByMemoizedSuffixPairDp (previously the
// test's own private helper), so a failure names the strategy that broke.
public sealed partial class MaximumLengthOfRepeatedSubarrayTests
{
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 2, 1], [3, 2, 1, 4, 7], 3 },
            { [0, 0, 0, 0, 0], [0, 0, 0, 0, 0], 5 },
            { [1, 2, 3], [4, 5, 6], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLengthByBruteForce_LeetCodeExamples_ReturnsLongestRepeatedRun(
        int[] first, int[] second, int expected)
    {
        var actual = MaximumLengthOfRepeatedSubarraySolution.FindLengthByBruteForce(first, second);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLengthByMemoizedSuffixPairDp_LeetCodeExamples_ReturnsLongestRepeatedRun(
        int[] first, int[] second, int expected)
    {
        var actual = MaximumLengthOfRepeatedSubarraySolution.FindLengthByMemoizedSuffixPairDp(first, second);

        Assert.Equal(expected, actual);
    }
}
