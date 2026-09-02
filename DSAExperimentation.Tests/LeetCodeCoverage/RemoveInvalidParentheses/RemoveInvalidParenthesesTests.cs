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
    public void RemoveByQueueBfs_LeetCodeExamples_ReturnsAllMinimalValidResults(string s, string[] expected) =>
        Assert.Equal(expected.Order(), RemoveInvalidParenthesesSolution.RemoveByQueueBfs(s).Order());

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveByBruteForceAllSubsets_LeetCodeExamples_ReturnsAllMinimalValidResults(string s, string[] expected) =>
        Assert.Equal(expected.Order(), RemoveInvalidParenthesesSolution.RemoveByBruteForceAllSubsets(s).Order());
}
