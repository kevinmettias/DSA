using DSAExperimentation.LeetCode.RemoveInvalidParentheses;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveInvalidParentheses;

// Harness only. Both strategies are RemoveInvalidParenthesesSolution's - this
// file just pins them to LeetCode's published examples.
public sealed class RemoveInvalidParenthesesTests
{
    public static TheoryData<string, string[]> Examples =>
        new()
        {
            { "()())()", ["(())()", "()()()"] },
            { "(a)())()", ["(a())()", "(a)()()"] },
            { ")(", [""] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveByQueueBfs_LeetCodeExamples_ReturnsAllMinimalValidResults(string text, string[] expected) =>
        Assert.Equal(expected.Order(), RemoveInvalidParenthesesSolution.RemoveByQueueBfs(text).Order());

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveByBruteForceAllSubsets_LeetCodeExamples_ReturnsAllMinimalValidResults(string text, string[] expected) =>
        Assert.Equal(expected.Order(), RemoveInvalidParenthesesSolution.RemoveByBruteForceAllSubsets(text).Order());
}
