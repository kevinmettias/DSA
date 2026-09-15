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
    public static bool IsMatchByGreedyTwoPointer(MatchedText text, WildcardPattern pattern)
    {
        var (matched, p) = ScanGreedily((Text: text.Text, Pattern: pattern.Text));
        if (!matched)
        {
            return false;
        }

        return IsOnlyStarsRemaining(pattern.Text, p);
    }

    // The scan itself: walk the text remembering the most recent '*' and the text
    // position it last matched, so a mismatch can retry that star against one more
    // text character rather than branching. Reports whether the text was consumed,
    // together with the pattern position the walk stopped at.
    private static (bool Matched, int Pattern) ScanGreedily((string Text, string Pattern) input)
    {
        var s = 0;
        var p = 0;
        var star = -1;
        var match = 0;
        while (s < input.Text.Length)
        {
            if (MatchesOneCharacter(input.Pattern, p, input.Text, s))
            {
                s++;
                p++;
            }
            else if (p < input.Pattern.Length && input.Pattern[p] == '*')
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
                return (false, p);
            }
        }

        return (true, p);
    }

    // The pattern's current character matches the text's current one when the
    // pattern still has a character there and it is either '?' or that same
    // character.
    private static bool MatchesOneCharacter(string pattern, int p, string text, int s)
        => p < pattern.Length && (pattern[p] == '?' || pattern[p] == text[s]);

    // With the text consumed, the pattern may only be finished off by stars - any
    // other character left over would have nothing left to match.
    private static bool IsOnlyStarsRemaining(string pattern, int p)
    {
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
    public static bool IsMatchByMemoizedDp(MatchedText text, WildcardPattern pattern) =>
        Memoizer.Memoize<(int Text, int Pattern), bool>((0, 0), new MatchFromEveryPosition(text, pattern));

    // The recurrence, named: at each (text, pattern) position the remaining pattern
    // either consumes the text character under it or, being a '*', is allowed to
    // consume none at all. The two strings being matched belong to the caller and
    // never vary during a run, so they travel in as constructor state.
    private sealed class MatchFromEveryPosition(MatchedText text, WildcardPattern pattern)
        : IRecurrence<(int Text, int Pattern), bool>
    {
        /// <inheritdoc/>
        public bool Replay((int Text, int Pattern) state, IRecurrence<(int Text, int Pattern), bool> rest)
        {
            var (i, j) = state;

            if (j == pattern.Text.Length)
            {
                return i == text.Text.Length;
            }

            if (pattern.Text[j] == '*')
            {
                return rest.Replay((i, j + 1), rest)
                    || (i < text.Text.Length && rest.Replay((i + 1, j), rest));
            }

            return i < text.Text.Length
                && (pattern.Text[j] == '?' || pattern.Text[j] == text.Text[i])
                && rest.Replay((i + 1, j + 1), rest);
        }
    }

    // The two sides of a wildcard match, named for what each is in this problem rather
    // than left as two adjacent `string` positions a caller could hand over the wrong
    // way round with the compiler none the wiser: `text` is the string being matched in
    // full, `pattern` the wildcard expression it must satisfy.
    internal readonly record struct MatchedText(string Text);

    internal readonly record struct WildcardPattern(string Text);
}
