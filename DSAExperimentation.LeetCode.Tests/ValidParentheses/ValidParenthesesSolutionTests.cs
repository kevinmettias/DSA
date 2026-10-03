using DSAExperimentation.LeetCode.ValidParentheses;

namespace DSAExperimentation.LeetCode.Tests.ValidParentheses;

// Harness only. The bracket-matching walk is ValidParenthesesSolution's - this
// file just pins it to LeetCode's published examples.
public sealed partial class ValidParenthesesSolutionTests
{
    public static TheoryData<BracketCase> Examples =>
        new()
        {
            { new BracketCase(Brackets: "([{}])", Expected: true) },
            { new BracketCase(Brackets: "(]", Expected: false) },
            { new BracketCase(Brackets: "((", Expected: false) },
            { new BracketCase(Brackets: "()", Expected: true) },
            { new BracketCase(Brackets: "()[]{}", Expected: true) },
            { new BracketCase(Brackets: "([)]", Expected: false) },
            { new BracketCase(Brackets: "", Expected: true) },
            { new BracketCase(Brackets: ")", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidByBracketStack_LeetCodeExamples_ReturnsWhetherProperlyNested(BracketCase example)
    {
        var isValid = ValidParenthesesSolution.IsValidByBracketStack(example.Brackets);

        Assert.Equal(example.Expected, isValid);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidByRepeatedPairRemoval_LeetCodeExamples_ReturnsWhetherProperlyNested(BracketCase example)
    {
        var isValid = ValidParenthesesSolution.IsValidByRepeatedPairRemoval(example.Brackets);

        Assert.Equal(example.Expected, isValid);
    }

    // The two arms are competing strategies for one question, so the interesting property is
    // that the naive arm is not merely correct on the published examples but agrees with the
    // scan on every one of them - including the malformed inputs an early-exit arm could get
    // right for the wrong reason.
    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValid_AgreeOnEveryExample(BracketCase example) =>
        Assert.Equal(
            ValidParenthesesSolution.IsValidByBracketStack(example.Brackets),
            ValidParenthesesSolution.IsValidByRepeatedPairRemoval(example.Brackets));

    // One LeetCode example: the bracket string, and whether every opening bracket is
    // closed by its own kind in the right order. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct BracketCase(string Brackets, bool Expected);
}
