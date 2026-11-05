using DSAExperimentation.LeetCode.Solutions.ValidParenthesisString;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ValidParenthesisString;

// Harness only. Both strategies are ValidParenthesisStringSolution's, so a
// failure names the strategy that broke.
public sealed partial class ValidParenthesisStringSolutionTests
{
    public static TheoryData<ValidityCase> Cases =>
        new()
        {
            { new ValidityCase(S: "(((", Expected: false) },
            { new ValidityCase(S: "", Expected: true) },
            { new ValidityCase(S: "*", Expected: true) },
            { new ValidityCase(S: ")(", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsValidStringByReachableOpenCountDp_Cases_MatchesExpectedValidity(ValidityCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            ValidParenthesisStringSolution.IsValidStringByReachableOpenCountDp(testCase.S));

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsValidStringByTwoIndexStackSweep_Cases_MatchesExpectedValidity(ValidityCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            ValidParenthesisStringSolution.IsValidStringByTwoIndexStackSweep(testCase.S));

    // One case: the string to validate and whether it can be made valid. The
    // row names both positions - a bare `bool` argument would read as "true" and say
    // nothing about what is true.
    public readonly record struct ValidityCase(string S, bool Expected);
}
