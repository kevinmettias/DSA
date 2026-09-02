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
    public static bool IsMatchByRecursion(string text, string pattern)
    {
        return MatchFrom(0, 0);

        bool MatchFrom(int textIndex, int patternIndex)
        {
            if (patternIndex == pattern.Length)
            {
                return textIndex == text.Length;
            }

            var firstMatches = textIndex < text.Length
                && (pattern[patternIndex] == text[textIndex] || pattern[patternIndex] == '.');

            if (patternIndex + 1 < pattern.Length && pattern[patternIndex + 1] == '*')
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
    public static bool IsMatchByMemoization(string text, string pattern)
    {
        return Memoizer.Memoize<(int Text, int Pattern), bool>((0, 0), MatchFrom);

        bool MatchFrom((int Text, int Pattern) state, Func<(int Text, int Pattern), bool> match)
        {
            var (textIndex, patternIndex) = state;

            if (patternIndex == pattern.Length)
            {
                return textIndex == text.Length;
            }

            var firstMatches = textIndex < text.Length
                && (pattern[patternIndex] == text[textIndex] || pattern[patternIndex] == '.');

            if (patternIndex + 1 < pattern.Length && pattern[patternIndex + 1] == '*')
            {
                return match((textIndex, patternIndex + StarTokenLength))
                    || (firstMatches && match((textIndex + 1, patternIndex)));
            }

            return firstMatches && match((textIndex + 1, patternIndex + 1));
        }
    }
}
