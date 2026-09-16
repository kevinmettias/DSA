using DSAExperimentation.LeetCode.ValidParenthesisString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidParenthesisString;

// Harness only. Both strategies are ValidParenthesisStringSolution's, so a
// failure names the strategy that broke.
public sealed class ValidParenthesisStringTests
{
    public static TheoryData<ValidityExample> Examples =>
        new()
        {
            { new ValidityExample(S: "()", Expected: true) },
            { new ValidityExample(S: "(*)", Expected: true) },
            { new ValidityExample(S: "(*))", Expected: true) },
            { new ValidityExample(S: "(((", Expected: false) },
            { new ValidityExample(S: "", Expected: true) },
            { new ValidityExample(S: "*", Expected: true) },
            { new ValidityExample(S: "(", Expected: false) },
            { new ValidityExample(S: ")(", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidStringByReachableOpenCountDp_Examples_MatchesExpectedValidity(ValidityExample example) =>
        Assert.Equal(
            example.Expected,
            ValidParenthesisStringSolution.IsValidStringByReachableOpenCountDp(example.S));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidStringByTwoIndexStackSweep_Examples_MatchesExpectedValidity(ValidityExample example) =>
        Assert.Equal(
            example.Expected,
            ValidParenthesisStringSolution.IsValidStringByTwoIndexStackSweep(example.S));

    // One LeetCode example: the string to validate and whether it can be made valid. The
    // row names both positions - a bare `bool` argument would read as "true" and say
    // nothing about what is true.
    public readonly record struct ValidityExample(string S, bool Expected);
}
