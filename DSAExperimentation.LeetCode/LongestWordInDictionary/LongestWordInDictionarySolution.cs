using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.LongestWordInDictionary;

// LeetCode 720. Longest Word in Dictionary: find the longest word buildable one
// character at a time from other words in the list (ties broken lexicographically
// smallest), where "buildable" means every prefix of the word is itself a complete
// word in the list.
//
// The two strategies differ in how "is this prefix a complete word" is answered -
// a linear scan of the whole word list per prefix, or a single O(1) HasValue check
// per step of one root-to-leaf walk over this repo's own LowercaseTrie<bool>.
internal static class LongestWordInDictionarySolution
{
    private const string EmptyPrefix = "";
    private const string NoQualifyingWord = "";

    // The textbook answer: for every word, verify each of its prefixes exists
    // ANYWHERE in the word list via a linear scan - no index at all.
    // O(words^2 * wordLength) vs. the trie walk's O(totalCharacters).
    public static string LongestWordByDictionaryScan(string[] words)
    {
        string? best = null;

        foreach (var word in words)
        {
            if (!IsBuildable(word, words))
            {
                continue;
            }

            if (best is null || IsBetter(word, best))
            {
                best = word;
            }
        }

        return best ?? NoQualifyingWord;
    }

    private static bool IsBuildable(string word, string[] words)
    {
        for (var len = 1; len <= word.Length; len++)
        {
            var prefix = word[..len];
            var found = false;

            foreach (var candidate in words)
            {
                if (candidate == prefix)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsBetter(string candidate, string best) =>
        candidate.Length > best.Length || (candidate.Length == best.Length && string.CompareOrdinal(candidate, best) < 0);

    // This repo's own LowercaseTrie<bool>: build one trie from the whole word list,
    // then walk it from the root descending only through nodes whose HasValue is
    // already true - a step is legal exactly when the prefix reached so far is
    // itself a complete dictionary word, which is this problem's own "built one
    // character at a time" rule.
    public static string LongestWordByLowercaseTrieWalk(string[] words)
    {
        var trie = BuildTrie(words);

        return FindLongestWord(trie.Root, EmptyPrefix, NoQualifyingWord);
    }

    private static LowercaseTrie<bool> BuildTrie(string[] words)
    {
        var trie = new LowercaseTrie<bool>();

        foreach (var word in words)
        {
            trie.Set(word, true);
        }

        return trie;
    }

    private static string FindLongestWord(LowercaseTrieNode<bool> node, string prefix, string best)
    {
        best = BetterOf(prefix, best);

        for (var i = 0; i < LowercaseTrieNode<bool>.AlphabetSize; i++)
        {
            best = VisitChild(node, i, prefix, best);
        }

        return best;
    }

    private static string VisitChild(LowercaseTrieNode<bool> node, int childIndex, string prefix, string best)
    {
        var child = node.Children[childIndex];

        if (child is null || !child.HasValue)
        {
            return best;
        }

        return FindLongestWord(child, prefix + (char)('a' + childIndex), best);
    }

    private static string BetterOf(string prefix, string best)
    {
        if (prefix.Length == 0)
        {
            return best;
        }

        return IsBetter(prefix, best) ? prefix : best;
    }
}
