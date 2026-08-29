using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidParentheses;

// LeetCode 20. Valid Parentheses: this repo's Stack<char> tracking open brackets,
// popped and matched against each closer in turn.
public sealed partial class ValidParenthesesTests
{
    private static readonly Dictionary<char, char> ClosingToOpening = new()
    {
        [')'] = '(',
        [']'] = '[',
        ['}'] = '{',
    };

    [Fact]
    public void IsValid_ProperlyNestedAndMatched_ReturnsTrue() => Assert.True(IsValid("([{}])"));

    [Fact]
    public void IsValid_MismatchedCloser_ReturnsFalse() => Assert.False(IsValid("(]"));

    [Fact]
    public void IsValid_UnclosedOpener_ReturnsFalse() => Assert.False(IsValid("(("));

    private static bool IsValid(string brackets)
    {
        var openers = new RepoCharStack();

        foreach (var ch in brackets)
        {
            if (!ClosingToOpening.TryGetValue(ch, out var expectedOpener))
            {
                openers.Push(ch);
                continue;
            }

            if (!openers.TryPop(out var actualOpener) || actualOpener != expectedOpener)
            {
                return false;
            }
        }

        return openers.Count == 0;
    }
}
