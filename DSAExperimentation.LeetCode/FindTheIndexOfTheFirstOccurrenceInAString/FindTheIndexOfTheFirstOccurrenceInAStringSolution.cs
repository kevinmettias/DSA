using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.FindTheIndexOfTheFirstOccurrenceInAString;

// LeetCode 28. Find the Index of the First Occurrence in a String: return the
// index of needle's first occurrence in haystack, or -1 if it never occurs.
//
// The composed strategy is exactly RollingHashSearch.FindAll's first hit - Rabin-
// Karp already answers "every occurrence", so this problem only needs the first
// one. The baseline is the BCL substring search the repo's own algorithm has to
// beat.
internal static class FindTheIndexOfTheFirstOccurrenceInAStringSolution
{
    // What you would write without this repo: BCL ordinal substring search.
    public static int IndexOfByStringIndexOf(string haystack, string needle) =>
        haystack.IndexOf(needle, StringComparison.Ordinal);

    public static int IndexOfByRollingHash(string haystack, string needle)
    {
        var matches = RollingHashSearch.FindAll(haystack, needle);

        return matches.Count == 0 ? -1 : matches[0];
    }
}
