using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.FindTheOccurrenceOfFirstAlmostEqualSubstring;

// LeetCode 3303. Find the Occurrence of First Almost Equal Substring: the
// smallest starting index of a length-pattern.Length window of s that can be
// turned into pattern by changing at most one character, or -1 if no window
// qualifies.
//
// A window at i qualifies iff the run of matching characters starting at i
// (reading forward) and the run of matching characters ending at i +
// pattern.Length - 1 (reading backward) together cover at least
// pattern.Length - 1 positions - the one gap left over, if any, is the single
// allowed change. Both runs are exactly what a Z-function gives for free once
// pattern is placed at the front of a combined buffer with a sentinel s and
// pattern's own alphabet (lowercase letters, per #3303's constraints) cannot
// contain: this repo's ZFunction stays sentinel-free internally (see its own
// doc comment) precisely because it cannot assume that in general, but a
// LeetCode caller that knows its problem's alphabet is free to make that
// choice itself.
internal static class FindTheOccurrenceOfFirstAlmostEqualSubstringSolution
{
    private const char Sentinel = '\0';

    // The textbook answer: for every window, count mismatches directly and
    // stop as soon as a second one appears. Correct on any input, but this is
    // the O(n * m) arm the Z-function strategy below has to beat.
    public static int IndexOfFirstAlmostEqualSubstringByBruteForce(SearchedText s, MatchPattern pattern)
    {
        var windowLength = pattern.Text.Length;

        for (var start = 0; start <= s.Text.Length - windowLength; start++)
        {
            if (CountsAsAlmostEqual(s, pattern, start))
            {
                return start;
            }
        }

        return LeetCodeAnswer.None;
    }

    private static bool CountsAsAlmostEqual(SearchedText s, MatchPattern pattern, int start)
    {
        var mismatches = 0;

        for (var offset = 0; offset < pattern.Text.Length; offset++)
        {
            if (s.Text[start + offset] != pattern.Text[offset] && ++mismatches > 1)
            {
                return false;
            }
        }

        return true;
    }

    // ForwardMatch[i] and BackwardMatch[i] come from two Z-function passes -
    // one over pattern + sentinel + s, one over the reverse of both - each
    // O(s.Length + pattern.Length), so the whole strategy is linear where the
    // brute force is quadratic.
    public static int IndexOfFirstAlmostEqualSubstringByZFunction(SearchedText s, MatchPattern pattern)
    {
        var windowLength = pattern.Text.Length;
        var forwardMatch = ForwardMatchLengths(s, pattern);
        var backwardMatch = BackwardMatchLengths(s, pattern);

        for (var start = 0; start <= s.Text.Length - windowLength; start++)
        {
            var windowEnd = start + windowLength - 1;

            if (forwardMatch[start] + backwardMatch[windowEnd] >= windowLength - 1)
            {
                return start;
            }
        }

        return LeetCodeAnswer.None;
    }

    // forwardMatch[i]: how far s[i..] matches pattern from its start, capped
    // at pattern.Length by the sentinel (ZFunction can never extend a match
    // across it, since Sentinel cannot occur in either string).
    private static int[] ForwardMatchLengths(SearchedText s, MatchPattern pattern)
    {
        var combined = pattern.Text + Sentinel + s.Text;
        var z = ZFunction.Compute(combined);
        var forwardMatch = new int[s.Text.Length];

        for (var i = 0; i < s.Text.Length; i++)
        {
            forwardMatch[i] = z[pattern.Text.Length + 1 + i];
        }

        return forwardMatch;
    }

    // backwardMatch[j]: how far s[..j] matches pattern from its end, found by
    // running the same forward pass over both strings reversed and mapping
    // each reversed position back to its original index.
    private static int[] BackwardMatchLengths(SearchedText s, MatchPattern pattern)
    {
        var combined = Reverse(pattern.Text) + Sentinel + Reverse(s.Text);
        var z = ZFunction.Compute(combined);
        var backwardMatch = new int[s.Text.Length];

        for (var t = 0; t < s.Text.Length; t++)
        {
            backwardMatch[s.Text.Length - 1 - t] = z[pattern.Text.Length + 1 + t];
        }

        return backwardMatch;
    }

    private static string Reverse(string value)
    {
        var chars = value.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    // The two ends of a window search, named for the roles they play in this problem
    // rather than left as two adjacent `string` positions a caller could hand over the
    // wrong way round with the compiler none the wiser. `s` is the text being scanned
    // for a qualifying window; `pattern` is the string each window must almost equal.
    internal readonly record struct SearchedText(string Text);

    internal readonly record struct MatchPattern(string Text);
}
