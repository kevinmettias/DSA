using WordTrie = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.Trie<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
        bool>,
    bool>;

namespace DSAExperimentation.LeetCode.Solutions.CountingWordsWithAGivenPrefix;

// LeetCode 2185. Counting Words With a Given Prefix: how many strings in `words` start
// with `pref`.
//
// Both strategies visit every word - the answer is a count, so neither gets to stop
// early - and differ only in how "does this word start with pref" is decided: a direct
// character comparison, or an insert-then-HasPrefix round trip through this repo's own
// trie over the lowercase letters. That is LC 1455's pairing
// (CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution) asked of every word instead
// of only up to the first match.
[LeetCodeQuestion("counting-words-with-a-given-prefix", LeetCodeAnswerOrder.AsWritten)]
internal static class CountingWordsWithAGivenPrefixSolution
{
    // The textbook answer: string.StartsWith per word. Deliberately written with nothing
    // but the BCL - it is the arm the Trie composition below has to justify itself
    // against.
    public static int CountWordsWithPrefixByStartsWithScan(string[] words, string pref)
    {
        var count = 0;

        foreach (var word in words)
        {
            if (word.StartsWith(pref, StringComparison.Ordinal))
            {
                count++;
            }
        }

        return count;
    }

    // This repo's own Trie: insert one candidate word, then ask HasPrefix - ImplementTrie's
    // "insert 'apple', then HasPrefix('app') is true" pairing (LC 208), re-run per word. A
    // fresh Trie per word is what keeps the question "does THIS word start with pref"
    // rather than "does any word so far".
    public static int CountWordsWithPrefixByTriePerWord(string[] words, string pref)
    {
        var count = 0;

        foreach (var word in words)
        {
            var trie = new WordTrie();
            trie.Set(word, true);

            if (trie.HasPrefix(pref))
            {
                count++;
            }
        }

        return count;
    }
}
