using DSAExperimentation.LeetCode.GenerateParentheses;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GenerateParentheses;

// Harness only. Both strategies are GenerateParenthesesSolution's - this file pins
// them to LeetCode's published examples, stated once. Both strategies choose '('
// before ')' at every step, so they produce the same combinations in the same order
// and can be checked with one exact-sequence assertion each.
public sealed partial class GenerateParenthesesTests
{
    public static TheoryData<int, string[]> Examples =>
        new()
        {
            { 3, ["((()))", "(()())", "(())()", "()(())", "()()()"] },
            { 1, ["()"] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GenerateByBacktracking_LeetCodeExamples_ReturnsAllBalancedCombinations(
        int pairs, string[] expected) =>
        Assert.Equal(expected, GenerateParenthesesSolution.GenerateByBacktracking(pairs));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GenerateByRecursiveSpecialized_LeetCodeExamples_ReturnsAllBalancedCombinations(
        int pairs, string[] expected) =>
        Assert.Equal(expected, GenerateParenthesesSolution.GenerateByRecursiveSpecialized(pairs));
}
