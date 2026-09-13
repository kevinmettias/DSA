using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.RotateString;

// LeetCode 796. Rotate String: can goal be obtained from s by some number of
// left-shifts?
//
// Both strategies rest on the same observation - every rotation of s is a
// contiguous window of s+s, so goal is a rotation iff the two are the same length
// and goal occurs somewhere inside s+s. They differ only in how that occurrence is
// looked for: a restart-on-mismatch scan, or this repo's KMP failure function.
internal static class RotateStringSolution
{
    // The textbook answer: double s, then scan every start position, comparing
    // character by character and restarting from goal's first character on any
    // mismatch. O(n*m). Deliberately plain BCL indexing - it is the arm the KMP
    // strategy below has to justify itself against.
    public static bool CanRotateByNaiveScan(string s, string goal)
    {
        if (s.Length != goal.Length)
        {
            return false;
        }

        return ContainsByRestartingScan(s + s, goal);
    }

    private static bool ContainsByRestartingScan(string text, string pattern)
    {
        for (var start = 0; start + pattern.Length <= text.Length; start++)
        {
            if (MatchesAt(text, pattern, start))
            {
                return true;
            }
        }

        return false;
    }

    private static bool MatchesAt(string text, string pattern, int start)
    {
        for (var offset = 0; offset < pattern.Length; offset++)
        {
            if (text[start + offset] != pattern[offset])
            {
                return false;
            }
        }

        return true;
    }

    // The same containment question handed to PrefixFunctionSearch, which never
    // re-scans a character of s+s: on a mismatch it falls back through goal's
    // failure function instead of restarting, so the whole search is O(n + m).
    public static bool CanRotateByPrefixFunction(string s, string goal)
    {
        if (s.Length != goal.Length)
        {
            return false;
        }

        return PrefixFunctionSearch.FindAll(s + s, goal).Count > 0;
    }
}
