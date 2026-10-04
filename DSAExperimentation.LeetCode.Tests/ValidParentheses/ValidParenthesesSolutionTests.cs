using DSAExperimentation.LeetCode.ValidParentheses;

namespace DSAExperimentation.LeetCode.Tests.ValidParentheses;

// Harness only. The bracket-matching walk is ValidParenthesesSolution's - this
// file just pins it to LeetCode's published examples and four strings of its own.
public sealed partial class ValidParenthesesSolutionTests
{
    public static TheoryData<BracketCase> Examples =>
        new()
        {
            // LeetCode examples 1-5.
            { new BracketCase(Brackets: "()", Expected: true) },
            { new BracketCase(Brackets: "()[]{}", Expected: true) },
            { new BracketCase(Brackets: "(]", Expected: false) },
            { new BracketCase(Brackets: "([])", Expected: true) },
            { new BracketCase(Brackets: "([)]", Expected: false) },

            // All three kinds nested, each closed by its own kind; two openers never
            // closed; the empty string, with nothing left open; a closer with no opener.
            { new BracketCase(Brackets: "([{}])", Expected: true) },
            { new BracketCase(Brackets: "((", Expected: false) },
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

    // One example: the bracket string, and whether every opening bracket is
    // closed by its own kind in the right order. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct BracketCase(string Brackets, bool Expected);
}
