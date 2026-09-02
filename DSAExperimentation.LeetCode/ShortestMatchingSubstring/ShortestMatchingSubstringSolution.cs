using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ShortestMatchingSubstring;

// LeetCode 3455. Shortest Matching Substring: p contains exactly two '*', each
// matching any (possibly empty) run of characters, splitting p into three literal
// parts p1*p2*p3. A substring of s matches p iff it starts with p1, ends with p3,
// and contains p2 somewhere between - so the shortest match is found by scanning
// every occurrence of p1 as a candidate start and, for each, greedily taking the
// earliest possible p2 occurrence after it and the earliest possible p3 occurrence
// after that. The greedy step is optimal for a fixed start: an earlier valid p2
// end can only push the earliest valid subsequent p3 earlier or equal, never later.
internal static class ShortestMatchingSubstringSolution
{
    public static int ShortestLengthByBruteForceIndexOf(string s, string p) =>
        ShortestLengthByBruteForceIndexOf(s, MatchPattern.Parse(p));

    // The textbook arm: BCL string.IndexOf walking one candidate occurrence of each
    // literal part at a time, deliberately without this repo's KMP/binary-search
    // primitives - the arm the composed strategy below has to justify itself
    // against. Repeated per-p1-occurrence IndexOf scans make this O(|s| * matches),
    // not the O(|s| log |s|) the KMP + binary-search composition guarantees.
    public static int ShortestLengthByBruteForceIndexOf(string s, MatchPattern pattern)
    {
        var best = LeetCodeAnswer.None;
        var prefixStart = IndexOfFrom(s, pattern.Prefix, 0);

        while (prefixStart >= 0)
        {
            var middleStart = IndexOfFrom(s, pattern.Middle, prefixStart + pattern.Prefix.Length);

            if (middleStart >= 0)
            {
                var suffixStart = IndexOfFrom(s, pattern.Suffix, middleStart + pattern.Middle.Length);

                if (suffixStart >= 0)
                {
                    var length = suffixStart + pattern.Suffix.Length - prefixStart;
                    best = best == LeetCodeAnswer.None ? length : Math.Min(best, length);
                }
            }

            prefixStart = IndexOfFrom(s, pattern.Prefix, prefixStart + 1);
        }

        return best;
    }

    // string.IndexOf(value, from) throws once `from` runs past text.Length instead
    // of just reporting "not found" - this keeps the caller's loop a plain while
    // condition instead of a bounds check at every call site.
    private static int IndexOfFrom(string text, string value, int from) =>
        from > text.Length ? -1 : text.IndexOf(value, from, StringComparison.Ordinal);

    public static int ShortestLengthByKmpBinarySearch(string s, string p) =>
        ShortestLengthByKmpBinarySearch(PatternOccurrences.Build(s, MatchPattern.Parse(p)));

    // This repo's own composition: PrefixFunctionSearch.FindAll already gives every
    // occurrence of each literal part in ascending order, so "earliest occurrence at
    // or after a floor" is exactly BinarySearch.LowerBound over that list - O(|s| +
    // |p|) to build the three occurrence lists once, then O(log |s|) per candidate
    // start instead of a fresh linear scan.
    public static int ShortestLengthByKmpBinarySearch(PatternOccurrences occurrences)
    {
        var pattern = occurrences.Pattern;
        var middleSequence = new ArraySequence<int>(occurrences.MiddleStarts);
        var suffixSequence = new ArraySequence<int>(occurrences.SuffixStarts);
        var best = LeetCodeAnswer.None;

        foreach (var prefixStart in occurrences.PrefixStarts)
        {
            var middleIndex = BinarySearch.LowerBound(middleSequence, prefixStart + pattern.Prefix.Length);

            if (middleIndex == occurrences.MiddleStarts.Length)
            {
                continue;
            }

            var middleStart = occurrences.MiddleStarts[middleIndex];
            var suffixIndex = BinarySearch.LowerBound(suffixSequence, middleStart + pattern.Middle.Length);

            if (suffixIndex == occurrences.SuffixStarts.Length)
            {
                continue;
            }

            var suffixStart = occurrences.SuffixStarts[suffixIndex];
            var length = suffixStart + pattern.Suffix.Length - prefixStart;
            best = best == LeetCodeAnswer.None ? length : Math.Min(best, length);
        }

        return best;
    }
}

// p split on its two '*' characters into the three literal parts either side must
// match - a witness meaningful only to this problem, so it lives in this folder
// rather than Domain/ (ARCHITECTURE.md §17.3).
internal readonly record struct MatchPattern(string Prefix, string Middle, string Suffix)
{
    public static MatchPattern Parse(string p)
    {
        var parts = p.Split('*');
        return new MatchPattern(parts[0], parts[1], parts[2]);
    }
}

// The hoisted-overload input for ShortestLengthByKmpBinarySearch: every occurrence
// (ascending, ties to PrefixFunctionSearch.FindAll's own guarantee) of each of
// pattern's three literal parts within s, computed once so a benchmark measures the
// greedy binary-search combination alone - mirroring how OpenTheLockBenchmarks
// hoists LockGraph.Build out of the search it measures.
internal readonly record struct PatternOccurrences(
    MatchPattern Pattern, int[] PrefixStarts, int[] MiddleStarts, int[] SuffixStarts)
{
    public static PatternOccurrences Build(string s, MatchPattern pattern) =>
        new(
            pattern,
            PrefixFunctionSearch.FindAll(s, pattern.Prefix).ToArray(),
            PrefixFunctionSearch.FindAll(s, pattern.Middle).ToArray(),
            PrefixFunctionSearch.FindAll(s, pattern.Suffix).ToArray());
}
