using DSAExperimentation.LeetCode.CountNumberOfBalancedPermutations;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountNumberOfBalancedPermutations;

// Harness only: the algorithms live in CountNumberOfBalancedPermutationsSolution.
// One test method per strategy over one shared set of LeetCode's own examples, so
// a failure names the strategy that broke (TwoSumTests precedent).
public sealed class CountNumberOfBalancedPermutationsTests
{
    public static TheoryData<string, long> Examples =>
        new()
        {
            { "123", 2 },
            { "112", 1 },
            { "12345", 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountBalancedPermutationsByBruteForce_LeetCodeExamples_ReturnsBalancedPermutationCount(
        string num, long expected)
    {
        var actual = CountNumberOfBalancedPermutationsSolution.CountBalancedPermutationsByBruteForce(num);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountBalancedPermutationsByDigitCountDp_LeetCodeExamples_ReturnsBalancedPermutationCount(
        string num, long expected)
    {
        var actual = CountNumberOfBalancedPermutationsSolution.CountBalancedPermutationsByDigitCountDp(num);
        Assert.Equal(expected, actual);
    }
}
