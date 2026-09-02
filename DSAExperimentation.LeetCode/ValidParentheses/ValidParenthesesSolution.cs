using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.ValidParentheses;

// LeetCode 20. Valid Parentheses: a string is valid when every closing bracket
// matches the most recently unmatched opening bracket - a single Stack<char> scan,
// pushing openers and popping to check each closer against the top the instant it
// is seen.
internal static class ValidParenthesesSolution
{
    private static readonly Dictionary<char, char> ClosingToOpening = new()
    {
        [')'] = '(',
        [']'] = '[',
        ['}'] = '{',
    };

    public static bool IsValidByBracketStack(string brackets)
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
