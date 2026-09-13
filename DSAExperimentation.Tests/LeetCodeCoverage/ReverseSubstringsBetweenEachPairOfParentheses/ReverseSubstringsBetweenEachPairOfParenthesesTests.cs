using DSAExperimentation.LeetCode.ReverseSubstringsBetweenEachPairOfParentheses;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReverseSubstringsBetweenEachPairOfParentheses;

// Harness only. Both strategies are
// ReverseSubstringsBetweenEachPairOfParenthesesSolution's - this file pins them to
// LeetCode's published examples plus the unbracketed input the original test
// carried, which is the only case where neither strategy reverses anything.
public sealed class ReverseSubstringsBetweenEachPairOfParenthesesTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            { "(abcd)", "dcba" },
            { "(u(love)i)", "iloveu" },
            { "(ed(et(oc))el)", "leetcode" },
            { "a(bcdefghijkl(mno)p)q", "apmnolkjihgfedcbq" },
            { "abc", "abc" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseParenthesesByRepeatedSplice_LeetCodeExamples_ReversesInnermostGroupsFirst(
        string input, string expected) =>
        Assert.Equal(
            expected,
            ReverseSubstringsBetweenEachPairOfParenthesesSolution.ReverseParenthesesByRepeatedSplice(input));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseParenthesesByCharBufferStack_LeetCodeExamples_ReversesInnermostGroupsFirst(
        string input, string expected) =>
        Assert.Equal(
            expected,
            ReverseSubstringsBetweenEachPairOfParenthesesSolution.ReverseParenthesesByCharBufferStack(input));
}
