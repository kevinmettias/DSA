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

    // The naive arm the single-scan stack is measured against: repeatedly delete any
    // adjacent matching pair and re-scan until nothing changes. Deleting adjacent
    // matched pairs never changes the verdict, so this agrees with the stack scan -
    // but every pass re-walks the whole remaining string, which is exactly what the
    // one-pass scan avoids.
    public static bool IsValidByRepeatedPairRemoval(string brackets)
    {
        var remaining = new List<char>(brackets);

        while (true)
        {
            var reduced = new List<char>(remaining.Count);
            var removed = false;

            for (var i = 0; i < remaining.Count; i++)
            {
                if (i + 1 < remaining.Count && IsMatchingPair(remaining[i], remaining[i + 1]))
                {
                    i++;
                    removed = true;
                    continue;
                }

                reduced.Add(remaining[i]);
            }

            if (!removed)
            {
                return remaining.Count == 0;
            }

            remaining = reduced;
        }
    }

    private static bool IsMatchingPair(char opener, char closer) =>
        (opener, closer) is ('(', ')') or ('[', ']') or ('{', '}');

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
