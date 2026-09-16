using DSAExperimentation.LeetCode.LongestValidParentheses;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestValidParentheses;

// Harness only: the algorithms live in LongestValidParenthesesSolution. One test
// method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke.
public sealed partial class LongestValidParenthesesTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "(()", 2 },
            { ")()())", 4 },
            { "", 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LengthByDynamicProgrammingArray_LeetCodeExamples_ReturnsLongestValidSpan(string value, int expected)
        => Assert.Equal(expected, LongestValidParenthesesSolution.LengthByDynamicProgrammingArray(value));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LengthByStackScan_LeetCodeExamples_ReturnsLongestValidSpan(string value, int expected)
        => Assert.Equal(expected, LongestValidParenthesesSolution.LengthByStackScan(value));
}
