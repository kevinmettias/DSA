using DSAExperimentation.LeetCode.Solutions.ValidParentheses;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ValidParentheses;

// Harness only. The bracket-matching walk is ValidParenthesesSolution's - this
// file just pins it to three strings of its own.
public sealed partial class ValidParenthesesSolutionTests
{
    public static TheoryData<BracketCase> Cases =>
        new()
        {
            // Two openers never closed; the empty string, with nothing left open; a
            // closer with no opener.
            { new BracketCase(Brackets: "((", Expected: false) },
            { new BracketCase(Brackets: "", Expected: true) },
            { new BracketCase(Brackets: ")", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsValidByBracketStack_Cases_ReturnsWhetherProperlyNested(BracketCase testCase)
    {
        var isValid = ValidParenthesesSolution.IsValidByBracketStack(testCase.Brackets);

        Assert.Equal(testCase.Expected, isValid);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsValidByRepeatedPairRemoval_Cases_ReturnsWhetherProperlyNested(BracketCase testCase)
    {
        var isValid = ValidParenthesesSolution.IsValidByRepeatedPairRemoval(testCase.Brackets);

        Assert.Equal(testCase.Expected, isValid);
    }

    // One case: the bracket string, and whether every opening bracket is
    // closed by its own kind in the right order. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct BracketCase(string Brackets, bool Expected);
}
