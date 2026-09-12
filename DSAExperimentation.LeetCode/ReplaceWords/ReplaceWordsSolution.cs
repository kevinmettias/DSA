using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.ReplaceWords;

// LeetCode 648. Replace Words: replace every sentence word with the shortest
// dictionary root it starts with, leaving words with no matching root unchanged.
//
// The two strategies differ in how they find that shortest root - scanning the whole
// dictionary and checking StartsWith for each candidate (every root has to be
// checked anyway, since the first match found is not guaranteed to be the shortest),
// vs. this repo's own LowercaseTrie<bool>, where the shortest root falls out for free
// by walking the word once and stopping at the first HasValue node reached.
internal static class ReplaceWordsSolution
{
    // The textbook answer: scan every root against every word with BCL StartsWith.
    // O(words * dictionarySize * rootLength) vs. the trie walk's O(words * wordLength).
    public static string ReplaceByDictionaryScan(string[] dictionary, string sentence)
    {
        var words = sentence.Split(' ');

        for (var i = 0; i < words.Length; i++)
        {
            words[i] = ShortestMatchingRoot(dictionary, words[i]);
        }

        return string.Join(' ', words);
    }

    private static string ShortestMatchingRoot(string[] dictionary, string word)
    {
        string? best = null;

        foreach (var root in dictionary)
        {
            if (word.StartsWith(root, StringComparison.Ordinal) && (best is null || root.Length < best.Length))
            {
                best = root;
            }
        }

        return best ?? word;
    }

    // This repo's own LowercaseTrie<bool>: every root is set true, and walking a
    // sentence word stops at the first HasValue node - the shortest matching root,
    // because the walk visits nodes in root-to-leaf order.
    public static string ReplaceByTrieWalk(string[] dictionary, string sentence)
    {
        var trie = new LowercaseTrie<bool>();

        foreach (var root in dictionary)
        {
            trie.Set(root, true);
        }

        var words = sentence.Split(' ');

        for (var i = 0; i < words.Length; i++)
        {
            words[i] = ShortestRootPrefix(trie, words[i]);
        }

        return string.Join(' ', words);
    }

    private static string ShortestRootPrefix(LowercaseTrie<bool> trie, string word)
    {
        var current = trie.Root;

        for (var i = 0; i < word.Length; i++)
        {
            var next = current.Children[word[i] - 'a'];

            if (next is null)
            {
                return word;
            }

            current = next;

            if (current.HasValue)
            {
                return word[..(i + 1)];
            }
        }

        return word;
    }
}
