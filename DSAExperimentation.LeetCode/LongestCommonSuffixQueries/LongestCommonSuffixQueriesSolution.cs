using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.LeetCode.LongestCommonSuffixQueries;

// LeetCode 3093. Longest Common Suffix Queries: for each wordsQuery[i], return the
// index into wordsContainer of the word with the longest common SUFFIX, breaking
// ties by shortest word and then earliest index.
//
// A common suffix is a common PREFIX of the reversed strings, so this reduces to
// "walk a trie of reversed wordsContainer entries as far as the reversed query
// matches, and read off the best word recorded at the deepest node reached" - the
// same suffix-as-reversed-prefix move SuffixArray/SuffixAutomaton document
// elsewhere in this repo, here composed with the existing character trie instead.
internal static class LongestCommonSuffixQueriesSolution
{
    public static int[] FindIndicesByBruteForce(string[] wordsContainer, string[] wordsQuery)
    {
        var result = new int[wordsQuery.Length];

        for (var q = 0; q < wordsQuery.Length; q++)
        {
            result[q] = BestBruteForceMatch(wordsContainer, wordsQuery[q]);
        }

        return result;
    }

    private static int BestBruteForceMatch(string[] wordsContainer, string query)
    {
        var bestIndex = 0;
        var bestSuffixLength = -1;

        for (var i = 0; i < wordsContainer.Length; i++)
        {
            var suffixLength = CommonSuffixLength(wordsContainer[i], query);

            if (suffixLength > bestSuffixLength ||
                (suffixLength == bestSuffixLength && wordsContainer[i].Length < wordsContainer[bestIndex].Length))
            {
                bestSuffixLength = suffixLength;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private static int CommonSuffixLength(string a, string b)
    {
        var i = a.Length - 1;
        var j = b.Length - 1;
        var length = 0;

        while (i >= 0 && j >= 0 && a[i] == b[j])
        {
            length++;
            i--;
            j--;
        }

        return length;
    }

    // Trie<int>.Set only ever marks the FINAL node of the key it is given - there is
    // no public way to touch every ancestor node in a single walk - so, unlike a
    // hand-rolled trie, populating every prefix depth costs one Set per depth
    // (O(word length) calls, each itself an O(depth) walk from the root: O(m^2) per
    // word of length m). That is the price of staying inside Trie<TValue>'s existing
    // public surface rather than reaching into TrieNode<TValue> (which Trie.cs keeps
    // private) or growing the primitive a new per-node-callback insert just for this
    // problem. Still asymptotically better than the brute force above for anything
    // but the smallest containers, and every call is Trie<int>'s own Set/TryGetValue.
    public static int[] FindIndicesByTrie(string[] wordsContainer, string[] wordsQuery)
    {
        var trie = BuildSuffixTrie(wordsContainer);

        return FindIndicesByTrie(trie, wordsQuery);
    }

    public static int[] FindIndicesByTrie(Trie<int> trie, string[] wordsQuery)
    {
        var result = new int[wordsQuery.Length];

        for (var q = 0; q < wordsQuery.Length; q++)
        {
            result[q] = DeepestMatchIndex(trie, wordsQuery[q]);
        }

        return result;
    }

    // Every prefix of every reversed word (i.e. every suffix of the original word)
    // is inserted with the index of the best word sharing it: words are processed in
    // index order and a candidate only overwrites the stored value when it is
    // STRICTLY shorter, so an untouched tie silently keeps the earliest index.
    public static Trie<int> BuildSuffixTrie(string[] wordsContainer)
    {
        var trie = new Trie<int>();

        trie.Set(string.Empty, ShortestThenEarliest(wordsContainer));

        for (var i = 0; i < wordsContainer.Length; i++)
        {
            var reversed = Reverse(wordsContainer[i]);

            for (var length = 1; length <= reversed.Length; length++)
            {
                var prefix = reversed[..length];

                if (!trie.TryGetValue(prefix, out var current) || IsShorterWord(i, current, wordsContainer))
                {
                    trie.Set(prefix, i);
                }
            }
        }

        return trie;
    }

    private static int ShortestThenEarliest(string[] wordsContainer)
    {
        var best = 0;

        for (var i = 1; i < wordsContainer.Length; i++)
        {
            if (wordsContainer[i].Length < wordsContainer[best].Length)
            {
                best = i;
            }
        }

        return best;
    }

    private static bool IsShorterWord(int candidate, int current, string[] wordsContainer) =>
        wordsContainer[candidate].Length < wordsContainer[current].Length;

    // Walks reversed-query prefixes from longest to shortest, stopping at the first
    // one the trie actually has a value for. Length 0 (the empty suffix, common to
    // every word) always hits, since BuildSuffixTrie sets it unconditionally.
    private static int DeepestMatchIndex(Trie<int> trie, string query)
    {
        var reversed = Reverse(query);

        for (var length = reversed.Length; length >= 0; length--)
        {
            if (trie.TryGetValue(reversed[..length], out var index))
            {
                return index;
            }
        }

        return 0;
    }

    private static string Reverse(string value)
    {
        var chars = value.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }
}
