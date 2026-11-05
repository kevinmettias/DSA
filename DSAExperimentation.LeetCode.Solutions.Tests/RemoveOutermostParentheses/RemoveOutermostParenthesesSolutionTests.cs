using DSAExperimentation.LeetCode.Solutions.RemoveOutermostParentheses;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RemoveOutermostParentheses;

// Harness only. Both strategies are RemoveOutermostParenthesesSolution's - this file
// just pins them to a single nested primitive. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class RemoveOutermostParenthesesSolutionTests
{
    public static TheoryData<OutermostParenthesesCase> Cases =>
        new()
        {
            { new OutermostParenthesesCase(S: "(()())", Expected: "()()") },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void RemoveOuterParenthesesByDepthCounter_Cases_StripsEachPrimitivesOutermostPair(
        OutermostParenthesesCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            RemoveOutermostParenthesesSolution.RemoveOuterParenthesesByDepthCounter(testCase.S));

    [Theory]
    [MemberData(nameof(Cases))]
    public void RemoveOuterParenthesesByOpenerStack_Cases_StripsEachPrimitivesOutermostPair(
        OutermostParenthesesCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            RemoveOutermostParenthesesSolution.RemoveOuterParenthesesByOpenerStack(testCase.S));

    // One case: the primitive string and what stripping every
    // primitive's outermost pair leaves behind. Both values are strings, so each is
    // named at every construction site and a row reads as the case it is rather than
    // as two positions a caller has to keep in order. Nested because it is only ever
    // used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct OutermostParenthesesCase(string S, string Expected);
}
