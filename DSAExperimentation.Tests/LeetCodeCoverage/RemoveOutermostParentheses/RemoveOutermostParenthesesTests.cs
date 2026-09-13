using DSAExperimentation.LeetCode.RemoveOutermostParentheses;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveOutermostParentheses;

// Harness only. Both strategies are RemoveOutermostParenthesesSolution's - this file
// just pins them to LeetCode's published examples, a single nested primitive, and the
// all-trivial-primitives case whose whole answer is the empty string.
public sealed class RemoveOutermostParenthesesTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            { "(()())(())", "()()()" },
            { "(()())(())(()(()))", "()()()()(())" },
            { "()()", "" },
            { "(()())", "()()" },
            { "(((())))", "((()))" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveOuterParenthesesByDepthCounter_LeetCodeExamples_StripsEachPrimitivesOutermostPair(
        string s, string expected) =>
        Assert.Equal(expected, RemoveOutermostParenthesesSolution.RemoveOuterParenthesesByDepthCounter(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveOuterParenthesesByOpenerStack_LeetCodeExamples_StripsEachPrimitivesOutermostPair(
        string s, string expected) =>
        Assert.Equal(expected, RemoveOutermostParenthesesSolution.RemoveOuterParenthesesByOpenerStack(s));
}
