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

            if (BeatsBestMatch(
                suffixLength,
                bestSuffixLength,
                new CandidateWord(wordsContainer[i]),
                new BestWord(wordsContainer[bestIndex])))
            {
                bestSuffixLength = suffixLength;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    // A candidate word takes over as the best match when it shares a longer suffix
    // with the query, or an equally long one while being the shorter word - the
    // problem's own tie-break. The two words are NOT interchangeable here: only the
    // candidate's length is ever compared against the incumbent's, so the pair is
    // named for which is which rather than left as two adjacent `string` positions.
    private static bool BeatsBestMatch(int suffixLength, int bestSuffixLength, CandidateWord candidate, BestWord best)
        => suffixLength > bestSuffixLength ||
            (suffixLength == bestSuffixLength && candidate.Text.Length < best.Text.Length);

    private static int CommonSuffixLength(string a, string b)
    {
        var left = new SuffixCursor(a, a.Length - 1);
        var right = new SuffixCursor(b, b.Length - 1);
        var length = 0;

        while (SharesCharacterAt(left, right))
        {
            length++;
            left = left with { Index = left.Index - 1 };
            right = right with { Index = right.Index - 1 };
        }

        return length;
    }

    // Walking towards the front, the two words share another suffix character while
    // both still have a position to compare and those positions agree. Each word
    // travels with the position being read in it - a position only means anything
    // against its own word - so the comparison takes one cursor per side rather than
    // a bare index and a bare word a caller could pair up wrongly.
    private static bool SharesCharacterAt(SuffixCursor left, SuffixCursor right)
        => left.Index >= 0 && right.Index >= 0 && left.Text[left.Index] == right.Text[right.Index];

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

    // The two words a best-match comparison weighs up: the candidate the scan is
    // currently looking at, and the incumbent it has to beat. They are told apart by
    // type because the tie-break reads them apart - the candidate is the one whose
    // length is tested against the incumbent's.
    internal readonly record struct CandidateWord(string Text);

    internal readonly record struct BestWord(string Text);

    // One word plus the position in it currently being compared, walked towards the
    // front. The position is only meaningful against its own word, so the two travel
    // as one value instead of as separate arguments a caller could pair up wrongly.
    internal readonly record struct SuffixCursor(string Text, int Index);
}
