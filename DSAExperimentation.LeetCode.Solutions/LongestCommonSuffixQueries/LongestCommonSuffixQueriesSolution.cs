using SuffixTrie = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.Trie<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.DenseTrieLayout,
        int>,
    int>;
using SuffixNode = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieNode<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.DenseTrieLayout,
        int>,
    int>;

namespace DSAExperimentation.LeetCode.Solutions.LongestCommonSuffixQueries;

// LeetCode 3093. Longest Common Suffix Queries: for each wordsQuery[i], return the
// index into wordsContainer of the word with the longest common SUFFIX, breaking
// ties by shortest word and then earliest index.
//
// A common suffix is a common PREFIX of the reversed strings, so this reduces to
// "walk a trie of reversed wordsContainer entries as far as the reversed query
// matches, and read off the best word recorded at the deepest node reached" - the
// same suffix-as-reversed-prefix move SuffixArray/SuffixAutomaton document
// elsewhere in this repo, here composed with the existing character trie instead.
[LeetCodeQuestion("longest-common-suffix-queries", LeetCodeAnswerOrder.AsWritten, Promise = typeof(LongestCommonSuffixQueriesPromise))]
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

            if (IsCandidateBetterMatch(
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
    private static bool IsCandidateBetterMatch(
        int suffixLength, int bestSuffixLength, CandidateWord candidate, BestWord best)
        => suffixLength > bestSuffixLength ||
            (suffixLength == bestSuffixLength && candidate.Text.Length < best.Text.Length);

    private static int CommonSuffixLength(string containerWord, string queryWord)
    {
        var left = new SuffixCursor(containerWord, containerWord.Length - 1);
        var right = new SuffixCursor(queryWord, queryWord.Length - 1);
        var length = 0;

        while (HasCommonCharacterAt(left, right))
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
    private static bool HasCommonCharacterAt(SuffixCursor left, SuffixCursor right)
        => left.Index >= 0 && right.Index >= 0 && left.Text[left.Index] == right.Text[right.Index];

    // This repo's own trie over the lowercase letters, holding every word reversed, so a common
    // suffix is a common path from the root. Each node keeps the best word through it - the
    // shortest, then the earliest - written as the word is walked in, one step per letter, and a
    // query answers with the value at the deepest node its letters, read backwards, reach:
    // O(total length) to build and O(query length) to ask.
    public static int[] FindIndicesByTrie(string[] wordsContainer, string[] wordsQuery)
    {
        var trie = BuildSuffixTrie(wordsContainer);

        return FindIndicesByTrie(trie, wordsQuery);
    }

    public static int[] FindIndicesByTrie(SuffixTrie trie, string[] wordsQuery)
    {
        var result = new int[wordsQuery.Length];

        for (var q = 0; q < wordsQuery.Length; q++)
        {
            result[q] = DeepestMatchIndex(trie, wordsQuery[q]);
        }

        return result;
    }

    // Words are walked in index order, and a node's word is replaced only by a STRICTLY shorter
    // one, so a tie keeps the earlier index. The root, the empty suffix every word shares, is kept
    // like any other node, so it ends up holding the shortest word overall.
    public static SuffixTrie BuildSuffixTrie(string[] wordsContainer)
    {
        var trie = new SuffixTrie();

        for (var i = 0; i < wordsContainer.Length; i++)
        {
            var word = wordsContainer[i];
            var node = trie.Root;
            KeepIfShorter(node, i, wordsContainer);

            for (var end = word.Length - 1; end >= 0; end--)
            {
                node = node.Children.GetOrAddChildFor(word[end]);
                KeepIfShorter(node, i, wordsContainer);
            }
        }

        return trie;
    }

    private static void KeepIfShorter(SuffixNode node, int candidate, string[] wordsContainer)
    {
        if (!node.HasValue || IsShorterWord(candidate, node.Value, wordsContainer))
        {
            node.HasValue = true;
            node.Value = candidate;
        }
    }

    private static bool IsShorterWord(int candidate, int current, string[] wordsContainer) =>
        wordsContainer[candidate].Length < wordsContainer[current].Length;

    // Follows the query from its last letter for as long as the trie does, and answers with the
    // deepest node's word. The root holds a word once any is built, so a query sharing no last
    // letter with any word is answered by the shortest word.
    private static int DeepestMatchIndex(SuffixTrie trie, string query)
    {
        var node = trie.Root;

        for (var end = query.Length - 1; end >= 0; end--)
        {
            var child = node.Children.ChildFor(query[end]);

            if (child is null)
            {
                break;
            }

            node = child;
        }

        return node.Value;
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
