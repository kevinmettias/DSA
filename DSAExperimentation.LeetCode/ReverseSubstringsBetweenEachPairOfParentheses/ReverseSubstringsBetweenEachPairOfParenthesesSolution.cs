using RepoCharListStack = DSAExperimentation.DataStructures.Stack.Stack<System.Collections.Generic.List<char>>;

namespace DSAExperimentation.LeetCode.ReverseSubstringsBetweenEachPairOfParentheses;

// LeetCode 1190. Reverse Substrings Between Each Pair of Parentheses: reverse each
// bracketed substring, starting from the innermost pair, and return the result with
// the brackets removed.
//
// The two strategies differ in what they carry between characters: the baseline
// keeps rebuilding the whole string once per pair, while the composed arm keeps one
// in-progress character buffer per open paren on this repo's own Stack<List<char>>,
// so a character is re-reversed only once per level that encloses it.
internal static class ReverseSubstringsBetweenEachPairOfParenthesesSolution
{
    // The textbook answer: find the first ')', scan back to its matching '(',
    // splice the reversed inner substring back in, repeat. Deliberately BCL-only,
    // and quadratic - the whole string is rebuilt once per parenthesis pair.
    public static string ReverseParenthesesByRepeatedSplice(string s)
    {
        var current = s;

        while (true)
        {
            var closeIndex = current.IndexOf(')');

            if (closeIndex < 0)
            {
                break;
            }

            var openIndex = current.LastIndexOf('(', closeIndex);
            var inner = current.Substring(openIndex + 1, closeIndex - openIndex - 1);
            var reversedInner = new string(inner.Reverse().ToArray());
            var beforeGroup = current.AsSpan(0, openIndex);
            var afterGroup = current.AsSpan(closeIndex + 1);
            current = string.Concat(beforeGroup, reversedInner, afterGroup);
        }

        return current;
    }

    // One pass, one buffer per nesting level on this repo's own Stack<List<char>>:
    // '(' pushes the enclosing buffer aside and starts a fresh one, ')' reverses the
    // current buffer and folds it into the one it closes back into. Linear in the
    // total number of characters times the nesting depth they sit under.
    public static string ReverseParenthesesByCharBufferStack(string s)
    {
        var groups = new RepoCharListStack();
        var current = new List<char>();

        foreach (var ch in s)
        {
            current = ProcessChar(groups, current, ch);
        }

        return new string(current.ToArray());
    }

    // One character folded into the buffer stack: '(' opens a fresh buffer above the
    // current one, ')' reverses the current buffer and appends it to the one it
    // closes back into, anything else just accumulates.
    private static List<char> ProcessChar(RepoCharListStack groups, List<char> current, char ch)
    {
        switch (ch)
        {
            case '(':
                groups.Push(current);
                return [];
            case ')':
                current.Reverse();

                if (groups.TryPop(out var enclosing))
                {
                    enclosing.AddRange(current);
                    return enclosing;
                }

                return current;
            default:
                current.Add(ch);
                return current;
        }
    }
}
