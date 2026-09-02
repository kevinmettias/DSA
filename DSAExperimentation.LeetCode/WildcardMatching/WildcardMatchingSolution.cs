using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.WildcardMatching;

// LeetCode 44. Wildcard Matching: does pattern (with '?' matching any one
// character and '*' matching any sequence, including empty) match text in full?
//
// The two strategies are the textbook greedy two-pointer scan - which remembers
// the most recent '*' and retries it against one more text character on a
// mismatch instead of exploring every split explicitly - and a memoized top-down
// recurrence over (text position, pattern position) composed from this repo's own
// Memoizer.
internal static class WildcardMatchingSolution
{
    // The textbook answer: track the most recent '*' (star) and the text position
    // it last matched (match), so a mismatch can retry the star against one more
    // text character rather than branching recursively. Deliberately written
    // without this repo's primitives - the arm the composed solution below has to
    // justify itself against.
    public static bool IsMatchByGreedyTwoPointer(string text, string pattern)
    {
        var s = 0;
        var p = 0;
        var star = -1;
        var match = 0;

        while (s < text.Length)
        {
            if (p < pattern.Length && (pattern[p] == '?' || pattern[p] == text[s]))
            {
                s++;
                p++;
            }
            else if (p < pattern.Length && pattern[p] == '*')
            {
                star = p++;
                match = s;
            }
            else if (star != -1)
            {
                p = star + 1;
                s = ++match;
            }
            else
            {
                return false;
            }
        }

        while (p < pattern.Length && pattern[p] == '*')
        {
            p++;
        }

        return p == pattern.Length;
    }

    // A '*' either matches zero characters (advance the pattern) or one more text
    // character (advance the text and retry the same pattern position); anything
    // else must match exactly. Composed from this repo's own Memoizer so every
    // (text, pattern) position pair is solved once.
    public static bool IsMatchByMemoizedDp(string text, string pattern)
    {
        return Memoizer.Memoize<(int Text, int Pattern), bool>((0, 0), MatchFrom);

        bool MatchFrom((int Text, int Pattern) state, Func<(int Text, int Pattern), bool> match)
        {
            var (i, j) = state;

            if (j == pattern.Length)
            {
                return i == text.Length;
            }

            if (pattern[j] == '*')
            {
                return match((i, j + 1)) || (i < text.Length && match((i + 1, j)));
            }

            return i < text.Length && (pattern[j] == '?' || pattern[j] == text[i]) && match((i + 1, j + 1));
        }
    }
}
