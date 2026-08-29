using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.Algorithms.StringMatching;

// Rabin-Karp string matching, built on DataStructures.RollingHash.RollingHash
// rather than a hand-rolled sliding-window rolling hash - a single source of
// truth for the collision-safety-sensitive modular arithmetic, at the cost of
// O(text.Length + pattern.Length) prefix-array memory instead of O(1)
// sliding-window state, the same "compose an existing Representation" trade-off
// IntervalSet already makes composing DynamicArray (ARCHITECTURE.md §4).
//
// Named for the mechanism plus "Search" (RollingHashSearch), matching
// PrefixFunctionSearch's own descriptive-name-over-eponymous convention -
// "Rabin-Karp" stays in this comment, not the type name.
//
// Expected O(n+m), not guaranteed like PrefixFunctionSearch's O(n+m): every hash
// match is verified with a real SequenceEqual before being reported (RollingHash's
// own equality-screen-not-oracle escape hatch), so FindAll is always correct, but
// a pathological input that made every window false-positive-hash-match could in
// principle degrade toward O(n*m). RollingHash's double-hashing default makes
// that practically unreachable, not merely unlikely - see RollingHash.cs's own
// doc comment for the collision-probability argument.
internal static class RollingHashSearch
{
    // Every starting index in text where pattern occurs, including overlapping
    // occurrences, in ascending order. An empty pattern matches at every
    // insertion point, 0..text.Length inclusive - the same convention
    // PrefixFunctionSearch.FindAll uses. A pattern longer than text can never
    // occur, so that returns an empty list rather than throwing: unlike
    // RollingHash.Hash's own out-of-range-query throw, "how many occurrences"
    // is always an answerable question, just sometimes answered zero.
    public static List<int> FindAll(ReadOnlySpan<char> text, ReadOnlySpan<char> pattern)
        => FindAll(text, pattern, EqualityComparer<char>.Default);

    public static List<int> FindAll(ReadOnlySpan<char> text, ReadOnlySpan<char> pattern, IEqualityComparer<char> comparer)
    {
        if (pattern.IsEmpty)
        {
            return MatchEveryInsertionPoint(text);
        }

        if (pattern.Length > text.Length)
        {
            return [];
        }

        var textHash = new RollingHash(text, comparer);
        var patternHash = new RollingHash(pattern, comparer);
        var target = patternHash.Hash(0, pattern.Length);
        var state = new SearchState(textHash, target, comparer);
        return MatchWindows(text, pattern, state);
    }

    private static List<int> MatchEveryInsertionPoint(ReadOnlySpan<char> text)
    {
        var matches = new List<int>();

        for (var position = 0; position <= text.Length; position++)
        {
            matches.Add(position);
        }

        return matches;
    }

    // Slides every window once, screening with the O(1) hash comparison first and
    // only paying for a real SequenceEqual on the (expected to be rare) windows
    // that pass the screen - the escape hatch from RollingHash's own doc comment,
    // applied for real.
    private static List<int> MatchWindows(ReadOnlySpan<char> text, ReadOnlySpan<char> pattern, SearchState state)
    {
        var matches = new List<int>();

        for (var start = 0; start <= text.Length - pattern.Length; start++)
        {
            var isHashMatch = state.TextHash.Hash(start, pattern.Length) == state.Target;
            var window = text.Slice(start, pattern.Length);
            var isRealMatch = isHashMatch && window.SequenceEqual(pattern, state.Comparer);

            if (isRealMatch)
            {
                matches.Add(start);
            }
        }

        return matches;
    }

    // Bundles the state MatchWindows needs beyond text/pattern/start - grouping
    // keeps MatchWindows at 3 parameters instead of 5, the same
    // parameter-count fix PrefixFunctionSearch's FailureFunctionMatcher already
    // makes for Advance. Private and un-fileworthy on its own.
    private readonly record struct SearchState(RollingHash TextHash, RollingHashValue Target, IEqualityComparer<char> Comparer);
}
