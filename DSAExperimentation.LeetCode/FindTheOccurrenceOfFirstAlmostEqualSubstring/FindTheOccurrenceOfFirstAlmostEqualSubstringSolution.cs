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
    public static int IndexOfFirstAlmostEqualSubstringByBruteForce(string s, string pattern)
    {
        var windowLength = pattern.Length;

        for (var start = 0; start <= s.Length - windowLength; start++)
        {
            if (CountsAsAlmostEqual(s, pattern, start))
            {
                return start;
            }
        }

        return LeetCodeAnswer.None;
    }

    private static bool CountsAsAlmostEqual(string s, string pattern, int start)
    {
        var mismatches = 0;

        for (var offset = 0; offset < pattern.Length; offset++)
        {
            if (s[start + offset] != pattern[offset] && ++mismatches > 1)
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
    public static int IndexOfFirstAlmostEqualSubstringByZFunction(string s, string pattern)
    {
        var windowLength = pattern.Length;
        var forwardMatch = ForwardMatchLengths(s, pattern);
        var backwardMatch = BackwardMatchLengths(s, pattern);

        for (var start = 0; start <= s.Length - windowLength; start++)
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
    private static int[] ForwardMatchLengths(string s, string pattern)
    {
        var combined = pattern + Sentinel + s;
        var z = ZFunction.Compute(combined);
        var forwardMatch = new int[s.Length];

        for (var i = 0; i < s.Length; i++)
        {
            forwardMatch[i] = z[pattern.Length + 1 + i];
        }

        return forwardMatch;
    }

    // backwardMatch[j]: how far s[..j] matches pattern from its end, found by
    // running the same forward pass over both strings reversed and mapping
    // each reversed position back to its original index.
    private static int[] BackwardMatchLengths(string s, string pattern)
    {
        var combined = Reverse(pattern) + Sentinel + Reverse(s);
        var z = ZFunction.Compute(combined);
        var backwardMatch = new int[s.Length];

        for (var t = 0; t < s.Length; t++)
        {
            backwardMatch[s.Length - 1 - t] = z[pattern.Length + 1 + t];
        }

        return backwardMatch;
    }

    private static string Reverse(string value)
    {
        var chars = value.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }
}
