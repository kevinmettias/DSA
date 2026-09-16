using DSAExperimentation.LeetCode.CountKReducibleNumbersLessThanN;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountKReducibleNumbersLessThanN;

// Harness only: the algorithms live in CountKReducibleNumbersLessThanNSolution.
// One test method per strategy over one shared set of LeetCode's own examples, so
// a failure names the strategy that broke (TwoSumTests precedent).
public sealed partial class CountKReducibleNumbersLessThanNTests
{
    public static TheoryData<string, int, int> Examples =>
        new()
        {
            { "111", 1, 3 },
            { "1000", 2, 6 },
            { "1", 3, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountKReducibleNumbersByBruteForce_LeetCodeExamples_ReturnsKReducibleCount(
        string binaryDigits, int maxSteps, int expected)
    {
        var actual = CountKReducibleNumbersLessThanNSolution.CountKReducibleNumbersByBruteForce(
            binaryDigits, maxSteps);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountKReducibleNumbersByPopcountCombinatorics_LeetCodeExamples_ReturnsKReducibleCount(
        string binaryDigits, int maxSteps, int expected)
    {
        var actual = CountKReducibleNumbersLessThanNSolution.CountKReducibleNumbersByPopcountCombinatorics(
            binaryDigits, maxSteps);
        Assert.Equal(expected, actual);
    }
}
