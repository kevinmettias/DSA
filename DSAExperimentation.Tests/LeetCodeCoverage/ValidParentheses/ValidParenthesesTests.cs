using DSAExperimentation.LeetCode.ValidParentheses;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidParentheses;

// Harness only. The bracket-matching walk is ValidParenthesesSolution's - this
// file just pins it to LeetCode's published examples.
public sealed partial class ValidParenthesesTests
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

    // One LeetCode example: the bracket string, and whether every opening bracket is
    // closed by its own kind in the right order. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct BracketCase(string Brackets, bool Expected);
}
