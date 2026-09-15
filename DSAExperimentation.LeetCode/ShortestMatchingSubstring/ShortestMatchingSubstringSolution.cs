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
    public static int ShortestLengthByBruteForceIndexOf(SourceText s, WildcardPattern p) =>
        ShortestLengthByBruteForceIndexOf(s, MatchPattern.Parse(p.Text));

    // The textbook arm: BCL string.IndexOf walking one candidate occurrence of each
    // literal part at a time, deliberately without this repo's KMP/binary-search
    // primitives - the arm the composed strategy below has to justify itself
    // against. Repeated per-p1-occurrence IndexOf scans make this O(|s| * matches),
    // not the O(|s| log |s|) the KMP + binary-search composition guarantees.
    public static int ShortestLengthByBruteForceIndexOf(SourceText s, MatchPattern pattern)
    {
        var best = LeetCodeAnswer.None;
        var prefixStart = IndexOfFrom(s, new LiteralPart(pattern.Prefix), 0);

        while (prefixStart >= 0)
        {
            var middleStart = IndexOfFrom(s, new LiteralPart(pattern.Middle), prefixStart + pattern.Prefix.Length);

            if (middleStart >= 0)
            {
                var suffixStart = IndexOfFrom(s, new LiteralPart(pattern.Suffix), middleStart + pattern.Middle.Length);

                if (suffixStart >= 0)
                {
                    var length = suffixStart + pattern.Suffix.Length - prefixStart;
                    best = best == LeetCodeAnswer.None ? length : Math.Min(best, length);
                }
            }

            prefixStart = IndexOfFrom(s, new LiteralPart(pattern.Prefix), prefixStart + 1);
        }

        return best;
    }

    // string.IndexOf(value, from) throws once `from` runs past text.Length instead
    // of just reporting "not found" - this keeps the caller's loop a plain while
    // condition instead of a bounds check at every call site.
    private static int IndexOfFrom(SourceText text, LiteralPart value, int from) =>
        from > text.Text.Length ? -1 : text.Text.IndexOf(value.Text, from, StringComparison.Ordinal);

    public static int ShortestLengthByKmpBinarySearch(SourceText s, WildcardPattern p)
    {
        var occurrences = PatternOccurrences.Build(s.Text, MatchPattern.Parse(p.Text));

        return ShortestLengthByKmpBinarySearch(occurrences);
    }

    // This repo's own composition: PrefixFunctionSearch.FindAll already gives every
    // occurrence of each literal part in ascending order, so "earliest occurrence at
    // or after a floor" is exactly BinarySearch.LowerBound over that list - O(|s| +
    // |p|) to build the three occurrence lists once, then O(log |s|) per candidate
    // start instead of a fresh linear scan.
    public static int ShortestLengthByKmpBinarySearch(PatternOccurrences occurrences)
    {
        var pattern = occurrences.Pattern;
        var middleStarts = new ArraySequence<int>(occurrences.MiddleStarts);
        var suffixStarts = new ArraySequence<int>(occurrences.SuffixStarts);
        var best = LeetCodeAnswer.None;

        foreach (var prefixStart in occurrences.PrefixStarts)
        {
            var length = ShortestLengthFrom(prefixStart, pattern, middleStarts, suffixStarts);

            if (length != LeetCodeAnswer.None)
            {
                best = best == LeetCodeAnswer.None ? length : Math.Min(best, length);
            }
        }

        return best;
    }

    // The shortest match anchored at prefixStart: the earliest middle occurrence at
    // or after the prefix, then the earliest suffix occurrence after that middle.
    // LeetCodeAnswer.None when either part has no occurrence left to use, which is
    // the greedy `continue` this arm used to take over the loop's own body.
    private static int ShortestLengthFrom(
        int prefixStart,
        MatchPattern pattern,
        ArraySequence<int> middleStarts,
        ArraySequence<int> suffixStarts)
    {
        var middleIndex = BinarySearch.LowerBound(middleStarts, prefixStart + pattern.Prefix.Length);

        if (middleIndex == middleStarts.Length)
        {
            return LeetCodeAnswer.None;
        }

        var middleStart = middleStarts.Get(middleIndex);
        var suffixIndex = BinarySearch.LowerBound(suffixStarts, middleStart + pattern.Middle.Length);

        if (suffixIndex == suffixStarts.Length)
        {
            return LeetCodeAnswer.None;
        }

        var suffixStart = suffixStarts.Get(suffixIndex);

        return suffixStart + pattern.Suffix.Length - prefixStart;
    }

    // LC 3455's three operand roles, each given the type that says which one it is.
    // `s` is the text a match is looked for in and `p` the pattern it is looked for by,
    // and each of p's three literal parts is what a substring search is pointed at -
    // but as three bare `string` positions nothing stopped a caller from handing the
    // text over as the pattern, or the pattern over as the fragment to find, and the
    // search would have answered the wrong question with no complaint from the
    // compiler.
    internal readonly record struct SourceText(string Text);

    // p as LeetCode hands it over: the whole pattern with both '*' wildcards still in
    // place, before MatchPattern.Parse splits it on them. Distinct from MatchPattern
    // because it is not yet three parts, and from SourceText because it is what the
    // source is searched for rather than what is searched.
    internal readonly record struct WildcardPattern(string Text);

    // One of the three literal fragments MatchPattern.Parse splits p into, and the unit
    // IndexOfFrom seeks inside a SourceText. It is the sought side of that search: the
    // two swapped asks whether the text occurs inside the fragment, which is not the
    // question either caller means to ask.
    internal readonly record struct LiteralPart(string Text);

    // p split on its two '*' characters into the three literal parts either side must
    // match - a witness meaningful only to this problem, so it lives in this folder
    // rather than Domain/ (ARCHITECTURE.md §17.3). Nested because it is this
    // solution's own intermediate: nothing outside the problem needs to import it.
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
}
