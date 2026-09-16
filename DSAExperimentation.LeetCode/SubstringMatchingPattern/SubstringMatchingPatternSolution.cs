using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.SubstringMatchingPattern;

// LeetCode 3407. Substring Matching Pattern: pattern contains exactly one '*', which can
// expand to any (possibly empty) sequence of characters. pattern is a substring of subject
// exactly when some occurrence of pattern's prefix (the part before '*') starts at or
// before where some occurrence of pattern's suffix (the part after '*') starts - the '*'
// itself absorbs whatever lies between them, so the two halves never need to be
// found at the same time, only at compatible positions. Choosing the EARLIEST
// prefix occurrence and the LATEST suffix occurrence is always at least as good as
// any other pair: moving the prefix later only raises the position the suffix must
// clear, and moving the suffix earlier only lowers it.
internal static class SubstringMatchingPatternSolution
{
    // The textbook answer: enumerate every substring of subject (O(n^2) of them) and test
    // each directly against prefix/suffix with plain BCL span comparisons -
    // deliberately without this repo's string-matching primitives, the arm the KMP
    // strategy below has to justify itself against.
    public static bool HasMatchByBruteForce(SubjectText subject, WildcardPattern pattern)
    {
        var (prefix, suffix) = SplitOnWildcard(pattern.Text);

        for (var start = 0; start <= subject.Text.Length; start++)
        {
            for (var end = start; end <= subject.Text.Length; end++)
            {
                var window = subject.Text.AsSpan(start, end - start);

                if (IsWildcardMatch(window, prefix, suffix))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsWildcardMatch(ReadOnlySpan<char> window, ReadOnlySpan<char> prefix, ReadOnlySpan<char> suffix)
        => window.Length >= prefix.Length + suffix.Length
            && window.StartsWith(prefix) && window.EndsWith(suffix);

    // This repo's own KMP search (Algorithms.StringMatching.PrefixFunctionSearch):
    // one O(subject.Text.Length + prefix.Length) pass locates every prefix occurrence, one more
    // every suffix occurrence, and the answer is a single comparison between the
    // earliest prefix start and the latest suffix start.
    public static bool HasMatchByPrefixFunctionSearch(SubjectText subject, WildcardPattern pattern)
    {
        var (prefix, suffix) = SplitOnWildcard(pattern.Text);

        var prefixStarts = PrefixFunctionSearch.FindAll(subject.Text, prefix);
        var suffixStarts = PrefixFunctionSearch.FindAll(subject.Text, suffix);

        if (prefixStarts.Count == 0 || suffixStarts.Count == 0)
        {
            return false;
        }

        var earliestPrefixStart = prefixStarts[0];
        var latestSuffixStart = suffixStarts[^1];

        return latestSuffixStart >= earliestPrefixStart + prefix.Length;
    }

    private static (string Prefix, string Suffix) SplitOnWildcard(string pattern)
    {
        var starIndex = pattern.IndexOf('*');
        return (pattern[..starIndex], pattern[(starIndex + 1)..]);
    }
}
