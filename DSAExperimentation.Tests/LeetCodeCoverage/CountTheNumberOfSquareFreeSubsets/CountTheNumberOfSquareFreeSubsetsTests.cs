using DSAExperimentation.LeetCode.CountTheNumberOfSquareFreeSubsets;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountTheNumberOfSquareFreeSubsets;

// Harness only: the algorithms live in CountTheNumberOfSquareFreeSubsetsSolution. One
// test method per strategy over one shared set of examples, so a failure names the
// strategy that broke (TwoSumTests precedent).
public sealed class CountTheNumberOfSquareFreeSubsetsTests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [3, 4, 4, 5], 3 }, // {3}, {5}, {3,5} - every subset touching a 4 has a repeated factor of 2
            { [1], 1 }, // {1} alone
            { [1, 1], 3 }, // {1a}, {1b}, {1a,1b} - 1 never breaks square-freeness
            { [4, 8, 16], 0 }, // every value already has a squared factor of 2 on its own
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForce_LeetCodeExamples_ReturnsSquareFreeSubsetCount(int[] nums, long expected)
    {
        var actual = CountTheNumberOfSquareFreeSubsetsSolution.CountByBruteForce(nums);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBitmaskMemo_LeetCodeExamples_ReturnsSquareFreeSubsetCount(int[] nums, long expected)
    {
        var actual = CountTheNumberOfSquareFreeSubsetsSolution.CountByBitmaskMemo(nums);
        Assert.Equal(expected, actual);
    }
}
