using DSAExperimentation.LeetCode.DifferentWaysToAddParentheses;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DifferentWaysToAddParentheses;

// Harness only. Both strategies are DifferentWaysToAddParenthesesSolution's - this
// file pins them to LeetCode's published examples.
public sealed class DifferentWaysToAddParenthesesTests
{
    public static TheoryData<string, int[]> Examples =>
        new()
        {
            { "2-1-1", [0, 2] },
            { "2*3-4*5", [-34, -14, -10, -10, 10] },
            { "7", [7] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DiffWaysToComputeByPlainRecursion_LeetCodeExamples_ReturnsAllGroupingResults(
        string expression, int[] expected) =>
        Assert.Equal(
            expected.Order(),
            DifferentWaysToAddParenthesesSolution.DiffWaysToComputeByPlainRecursion(expression).Order());

    [Theory]
    [MemberData(nameof(Examples))]
    public void DiffWaysToComputeByMemoizedSubstring_LeetCodeExamples_ReturnsAllGroupingResults(
        string expression, int[] expected) =>
        Assert.Equal(
            expected.Order(),
            DifferentWaysToAddParenthesesSolution.DiffWaysToComputeByMemoizedSubstring(expression).Order());
}
