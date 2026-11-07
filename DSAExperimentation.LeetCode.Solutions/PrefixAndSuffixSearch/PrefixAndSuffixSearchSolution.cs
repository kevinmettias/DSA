using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.Solutions.PrefixAndSuffixSearch;

// LeetCode 745. Prefix and Suffix Search: given a dictionary of words, answer
// f(prefix, suffix) with the largest index of a word that has that prefix AND that
// suffix, or -1 if none does. The two strategies are the classic per-query-scan vs.
// precompute-once tradeoff.
//
// LeetCode's shape is a WordFilter built from the words and queried by f calls, so
// each strategy is a factory, CreateBy<Strategy>, taking only the words and making
// one: the object the catalog harness replays LeetCode's call scripts on
// (ARCHITECTURE 17.13). The strategy's algorithm lives in a nested class named for
// it - LinearScan.Search, and PrecomputedHashMap's BuildPrefixSuffixIndex and
// Search - which the object asks and the hand-written benchmark calls directly, so
// each strategy is written once.
[LeetCodeQuestion("prefix-and-suffix-search", LeetCodeAnswerOrder.AsWritten, Calls = typeof(PrefixAndSuffixSearchCalls))]
internal static class PrefixAndSuffixSearchSolution
{
    // LeetCode's shape: a WordFilter that keeps the words as given and scans them all
    // on every query.
    public static IWordFilter CreateByLinearScan(string[] words) => new LinearScanFilter(words);

    // LeetCode's shape: a WordFilter whose constructor pays the one-time
    // O(numWords * wordLength^2) precompute of every (prefix, suffix) substring pair
    // of each word into this repo's own HashMap<string,int>, later words overwriting
    // earlier ones on a shared key - so a hit always reports the largest matching
    // word index - after which every query is a single O(1)-average lookup.
    public static IWordFilter CreateByPrecomputedHashMap(string[] words) => new PrecomputedHashMapFilter(words);

    // The linear-scan strategy's one query.
    internal static class LinearScan
    {
        // The textbook baseline: scan every word for every query. Iterating words in
        // index order and letting a later match overwrite an earlier one is exactly
        // LeetCode's own "largest index" tie-break, achieved by iteration order alone.
        public static int Search(string[] words, SearchPrefix prefix, SearchSuffix suffix)
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
    }

    // The precomputed-hash-map strategy: the index its constructor builds once, and
    // one query against it.
    internal static class PrecomputedHashMap
    {
        private const string PrefixSuffixSeparator = "#";

        // The prepared-input query: one lookup in the index the constructor built.
        public static int Search(
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

    // The words as given; every query scans them all.
    private sealed class LinearScanFilter(string[] words) : IWordFilter
    {
        public int Search(SearchPrefix prefix, SearchSuffix suffix) => LinearScan.Search(words, prefix, suffix);
    }

    // Every word's (prefix, suffix) keys, indexed once; every query is one lookup.
    private sealed class PrecomputedHashMapFilter(string[] words) : IWordFilter
    {
        private readonly HashMap<string, int> _indexByPrefixAndSuffix = PrecomputedHashMap.BuildPrefixSuffixIndex(words);

        public int Search(SearchPrefix prefix, SearchSuffix suffix) =>
            PrecomputedHashMap.Search(_indexByPrefixAndSuffix, prefix, suffix);
    }
}
