using DSAExperimentation.LeetCode.Solutions.ScoreOfParentheses;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ScoreOfParentheses;

// Harness only. Both scoring strategies are ScoreOfParenthesesSolution's - this
// file just pins them to the mixed
// nesting/sibling shapes that separate "2 * inner" from "inner + inner", including
// the O(n^2) depth-rescan baseline that used to live unasserted in the benchmark.
public sealed partial class ScoreOfParenthesesSolutionTests
{
    public static TheoryData<string, int> Cases =>
        new()
        {
            { "()(())()", 4 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ScoreByNestedDepthScan_Cases_ReturnsBalancedStringScore(
        string parentheses, int expected) =>
        Assert.Equal(expected, ScoreOfParenthesesSolution.ScoreByNestedDepthScan(parentheses));

    [Theory]
    [MemberData(nameof(Cases))]
    public void ScoreByMonotonicStackFold_Cases_ReturnsBalancedStringScore(
        string parentheses, int expected) =>
        Assert.Equal(expected, ScoreOfParenthesesSolution.ScoreByMonotonicStackFold(parentheses));
}
