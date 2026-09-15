using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.PrefixAndSuffixSearch;

// LeetCode 745. Prefix and Suffix Search: given a dictionary of words, answer
// f(prefix, suffix) with the largest index of a word that has that prefix AND that
// suffix, or -1 if none does. The two strategies are the classic per-query-scan vs.
// precompute-once tradeoff.
internal static class PrefixAndSuffixSearchSolution
{
    private const string PrefixSuffixSeparator = "#";

    // The textbook baseline: scan every word for every query. Iterating words in
    // index order and letting a later match overwrite an earlier one is exactly
    // LeetCode's own "largest index" tie-break, achieved by iteration order alone.
    public static int SearchByLinearScan(string[] words, SearchPrefix prefix, SearchSuffix suffix)
    {
        var found = LeetCodeAnswer.None;

        for (var i = 0; i < words.Length; i++)
        {
            if (words[i].StartsWith(prefix.Text, StringComparison.Ordinal) &&
                words[i].EndsWith(suffix.Text, StringComparison.Ordinal))
            {
                found = i;
            }
        }

        return found;
    }

    // One-time O(numWords * wordLength^2) precompute of every (prefix, suffix)
    // substring pair of each word into this repo's own HashMap<string,int>, later
    // words overwriting earlier ones on a shared key - so a hit always reports the
    // largest matching word index - after which every query is a single
    // O(1)-average lookup.
    public static int SearchByPrecomputedHashMap(string[] words, SearchPrefix prefix, SearchSuffix suffix)
    {
        var index = BuildPrefixSuffixIndex(words);

        return SearchByPrecomputedHashMap(index, prefix, suffix);
    }

    public static int SearchByPrecomputedHashMap(
        HashMap<string, int> indexByPrefixAndSuffix, SearchPrefix prefix, SearchSuffix suffix)
        => indexByPrefixAndSuffix.TryGetValue(prefix.Text + PrefixSuffixSeparator + suffix.Text, out var index)
            ? index
            : LeetCodeAnswer.None;

    public static HashMap<string, int> BuildPrefixSuffixIndex(string[] words)
    {
        var indexByPrefixAndSuffix = new HashMap<string, int>();

        for (var wordIndex = 0; wordIndex < words.Length; wordIndex++)
        {
            var word = words[wordIndex];

            for (var prefixLength = 0; prefixLength <= word.Length; prefixLength++)
            {
                for (var suffixLength = 0; suffixLength <= word.Length; suffixLength++)
                {
                    var key = word[..prefixLength] + PrefixSuffixSeparator + word[(word.Length - suffixLength)..];
                    indexByPrefixAndSuffix.Set(key, wordIndex);
                }
            }
        }

        return indexByPrefixAndSuffix;
    }
}
