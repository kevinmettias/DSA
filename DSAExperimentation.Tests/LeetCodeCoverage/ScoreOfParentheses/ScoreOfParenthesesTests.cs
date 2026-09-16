using DSAExperimentation.LeetCode.ScoreOfParentheses;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ScoreOfParentheses;

// Harness only. Both scoring strategies are ScoreOfParenthesesSolution's - this
// file just pins them to LeetCode's published examples plus the mixed
// nesting/sibling shapes that separate "2 * inner" from "inner + inner", including
// the O(n^2) depth-rescan baseline that used to live unasserted in the benchmark.
public sealed partial class ScoreOfParenthesesTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "()", 1 },
            { "(())", 2 },
            { "()()", 2 },
            { "((()))", 4 },
            { "(()())", 4 },
            { "(()(()))", 6 },
            { "()(())()", 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ScoreByNestedDepthScan_LeetCodeExamples_ReturnsBalancedStringScore(
        string parentheses, int expected) =>
        Assert.Equal(expected, ScoreOfParenthesesSolution.ScoreByNestedDepthScan(parentheses));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ScoreByMonotonicStackFold_LeetCodeExamples_ReturnsBalancedStringScore(
        string parentheses, int expected) =>
        Assert.Equal(expected, ScoreOfParenthesesSolution.ScoreByMonotonicStackFold(parentheses));
}
