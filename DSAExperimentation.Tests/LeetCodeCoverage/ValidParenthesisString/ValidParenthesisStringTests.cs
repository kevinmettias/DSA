using static DSAExperimentation.LeetCode.ValidParenthesisString.ValidParenthesisStringSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidParenthesisString;

// Harness only. Both strategies are ValidParenthesisStringSolution's, so a
// failure names the strategy that broke.
public sealed class ValidParenthesisStringTests
{
    public static TheoryData<string, bool> Examples =>
        new()
        {
            { "()", true },
            { "(*)", true },
            { "(*))", true },
            { "(((", false },
            { "", true },
            { "*", true },
            { "(", false },
            { ")(", false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckValidStringByReachableOpenCountDp_Examples_MatchesExpectedValidity(string s, bool expected) =>
        Assert.Equal(expected, CheckValidStringByReachableOpenCountDp(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckValidStringByTwoIndexStackSweep_Examples_MatchesExpectedValidity(string s, bool expected) =>
        Assert.Equal(expected, CheckValidStringByTwoIndexStackSweep(s));
}
