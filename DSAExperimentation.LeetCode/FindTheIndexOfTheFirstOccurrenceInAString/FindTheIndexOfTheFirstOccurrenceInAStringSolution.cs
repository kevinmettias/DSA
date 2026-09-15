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
    public static int IndexOfByStringIndexOf(Haystack haystack, Needle needle) =>
        haystack.Text.IndexOf(needle.Text, StringComparison.Ordinal);

    public static int IndexOfByRollingHash(Haystack haystack, Needle needle)
    {
        var matches = RollingHashSearch.FindAll(haystack.Text, needle.Text);

        return matches.Count == 0 ? -1 : FirstMatchIndex(matches);
    }

    private static int FirstMatchIndex(List<int> matches) => matches[0];

    // The two sides of a substring search, named for what each is in this problem
    // rather than left as two adjacent `string` positions a caller could hand over the
    // wrong way round with the compiler none the wiser: `haystack` is the text being
    // searched, `needle` the substring being looked for inside it.
    internal readonly record struct Haystack(string Text);

    internal readonly record struct Needle(string Text);
}
