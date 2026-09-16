using DSAExperimentation.LeetCode.RemoveOutermostParentheses;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveOutermostParentheses;

// Harness only. Both strategies are RemoveOutermostParenthesesSolution's - this file
// just pins them to LeetCode's published examples, a single nested primitive, and the
// all-trivial-primitives case whose whole answer is the empty string.
public sealed partial class RemoveOutermostParenthesesTests
{
    public static TheoryData<OutermostParenthesesCase> Examples =>
        new()
        {
            { new OutermostParenthesesCase(S: "(()())(())", Expected: "()()()") },
            { new OutermostParenthesesCase(S: "(()())(())(()(()))", Expected: "()()()()(())") },
            { new OutermostParenthesesCase(S: "()()", Expected: "") },
            { new OutermostParenthesesCase(S: "(()())", Expected: "()()") },
            { new OutermostParenthesesCase(S: "(((())))", Expected: "((()))") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveOuterParenthesesByDepthCounter_LeetCodeExamples_StripsEachPrimitivesOutermostPair(
        OutermostParenthesesCase example) =>
        Assert.Equal(
            example.Expected,
            RemoveOutermostParenthesesSolution.RemoveOuterParenthesesByDepthCounter(example.S));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveOuterParenthesesByOpenerStack_LeetCodeExamples_StripsEachPrimitivesOutermostPair(
        OutermostParenthesesCase example) =>
        Assert.Equal(
            example.Expected,
            RemoveOutermostParenthesesSolution.RemoveOuterParenthesesByOpenerStack(example.S));

    // One LeetCode example: the primitive string and what stripping every
    // primitive's outermost pair leaves behind. Both values are strings, so each is
    // named at every construction site and a row reads as the case it is rather than
    // as two positions a caller has to keep in order. Nested because it is only ever
    // used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct OutermostParenthesesCase(string S, string Expected);
}
