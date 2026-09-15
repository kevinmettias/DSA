using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.RegularExpressionMatching;

// LeetCode 10. Regular Expression Matching: does `pattern` (with '.' matching any
// single character and '*' matching zero or more of the preceding element) match
// the whole of `text`? Both strategies share the same (textIndex, patternIndex)
// recurrence; they differ only in whether repeated states are cached.
internal static class RegularExpressionMatchingSolution
{
    private const int StarTokenLength = 2;

    // Baseline: uncached recursive branching over the (textIndex, patternIndex)
    // state space - exponential on a pattern like "a*a*a*...b" against a text
    // with no trailing 'b' - deliberately written without this repo's primitives.
    public static bool IsMatchByRecursion(SubjectText text, RegexPattern pattern)
    {
        return MatchFrom(0, 0);

        bool MatchFrom(int textIndex, int patternIndex)
        {
            if (patternIndex == pattern.Text.Length)
            {
                return textIndex == text.Text.Length;
            }

            var firstMatches = textIndex < text.Text.Length
                && (pattern.Text[patternIndex] == text.Text[textIndex] || pattern.Text[patternIndex] == '.');

            if (patternIndex + 1 < pattern.Text.Length && pattern.Text[patternIndex + 1] == '*')
            {
                return MatchFrom(textIndex, patternIndex + StarTokenLength)
                    || (firstMatches && MatchFrom(textIndex + 1, patternIndex));
            }

            return firstMatches && MatchFrom(textIndex + 1, patternIndex + 1);
        }
    }

    // The same recurrence routed through this repo's Memoizer, caching on
    // (textIndex, patternIndex) so shared suffixes are solved once instead of
    // re-branching every time they're reached.
    public static bool IsMatchByMemoization(SubjectText text, RegexPattern pattern) =>
        Memoizer.Memoize<(int Text, int Pattern), bool>((0, 0), new PatternMatchFrom(text, pattern));

    // The recurrence itself, named: whether the pattern from `patternIndex` matches
    // the text from `textIndex`. It reads the two operands it was constructed with
    // rather than closing over either of them as a captured local.
    private sealed class PatternMatchFrom(SubjectText text, RegexPattern pattern)
        : IRecurrence<(int Text, int Pattern), bool>
    {
        public bool Replay((int Text, int Pattern) state, IRecurrence<(int Text, int Pattern), bool> rest)
        {
            var (textIndex, patternIndex) = state;

            if (patternIndex == pattern.Text.Length)
            {
                return textIndex == text.Text.Length;
            }

            var firstMatches = textIndex < text.Text.Length
                && (pattern.Text[patternIndex] == text.Text[textIndex] || pattern.Text[patternIndex] == '.');

            if (patternIndex + 1 < pattern.Text.Length && pattern.Text[patternIndex + 1] == '*')
            {
                return rest.Replay((textIndex, patternIndex + StarTokenLength), rest)
                    || (firstMatches && rest.Replay((textIndex + 1, patternIndex), rest));
            }

            return firstMatches && rest.Replay((textIndex + 1, patternIndex + 1), rest);
        }
    }

    // LC 10's two operands, named for the roles they play here rather than left as two
    // adjacent `string` positions a caller could hand over the wrong way round with the
    // compiler none the wiser. `text` is the string being tested and `pattern` the
    // regular expression it is tested against - asking whether the pattern matches the
    // text is not the reverse question, and the recurrence walks the two indices at
    // different rates.
    internal readonly record struct SubjectText(string Text);

    internal readonly record struct RegexPattern(string Text);
}
