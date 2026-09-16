using DSAExperimentation.LeetCode.ReverseSubstringsBetweenEachPairOfParentheses;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReverseSubstringsBetweenEachPairOfParentheses;

// Harness only. Both strategies are
// ReverseSubstringsBetweenEachPairOfParenthesesSolution's - this file pins them to
// LeetCode's published examples plus the unbracketed input the original test
// carried, which is the only case where neither strategy reverses anything.
public sealed partial class ReverseSubstringsBetweenEachPairOfParenthesesTests
{
    public static TheoryData<ParenthesesExample> Examples =>
        new()
        {
            new ParenthesesExample(Input: "(abcd)", Expected: "dcba"),
            new ParenthesesExample(Input: "(u(love)i)", Expected: "iloveu"),
            new ParenthesesExample(Input: "(ed(et(oc))el)", Expected: "leetcode"),
            new ParenthesesExample(Input: "a(bcdefghijkl(mno)p)q", Expected: "apmnolkjihgfedcbq"),
            new ParenthesesExample(Input: "abc", Expected: "abc"),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseParenthesesByRepeatedSplice_LeetCodeExamples_ReversesInnermostGroupsFirst(
        ParenthesesExample example) =>
        Assert.Equal(
            example.Expected,
            ReverseSubstringsBetweenEachPairOfParenthesesSolution.ReverseParenthesesByRepeatedSplice(example.Input));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseParenthesesByCharBufferStack_LeetCodeExamples_ReversesInnermostGroupsFirst(
        ParenthesesExample example) =>
        Assert.Equal(
            example.Expected,
            ReverseSubstringsBetweenEachPairOfParenthesesSolution.ReverseParenthesesByCharBufferStack(example.Input));

    // One LeetCode example: the bracketed input and the string it reverses to. Both
    // are strings, and at the call site they sit in adjacent positions, so the
    // bundle names the input and the expectation rather than leaving them swappable.
    public readonly record struct ParenthesesExample(string Input, string Expected);
}
