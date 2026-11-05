using DSAExperimentation.Domain.Language.English;
using WordTrie = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.Trie<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
        bool>,
    bool>;

namespace DSAExperimentation.LeetCode.Solutions.ImplementTrie;

// LeetCode 208. Implement Trie (Prefix Tree): a design problem - insert/search/
// startsWith against a fixed word set, not a single return value.
//
// This repo's own trie already implements exactly this shape
// (Set/HasKey/HasPrefix, LeetCode's own verbs traded for this repo's naming - the
// same trade LRUCacheSolution makes over ICache<TKey,TValue>), so the composed
// strategy is that trie, directly, over the lowercase letters LC 208's words use.
// The textbook strategy is the node chain LC 208's own editorial writes - each node
// 26 child slots and an end-of-word mark, over plain arrays - asked HasWord and
// HasPrefix in place of LeetCode's search and startsWith, as this repo's own Trie is
// asked HasKey and HasPrefix.
internal static class ImplementTrieSolution
{
    // The textbook answer, the arm this repo's Trie is measured against (ARCHITECTURE 17.5).
    public static ArrayNodeTrie CreateByArrayNodes() => new();

    public static WordTrie CreateByTriePrimitive() => new();

    internal sealed class ArrayNodeTrie
    {

        private readonly Node _root = new();

        public void Insert(string word)
        {
            var node = _root;

            foreach (var letter in word)
            {
                node = node.Children[letter - EnglishLowercase.FirstLetter] ??= new Node();
            }

            node.IsWord = true;
        }

        public bool HasWord(string word)
        {
            var node = NodeAt(word);

            return node is not null && node.IsWord;
        }

        public bool HasPrefix(string prefix) => NodeAt(prefix) is not null;

        // The node `text` spells from the root, or null where the chain stops short of it.
        private Node? NodeAt(string text)
        {
            Node? node = _root;

            foreach (var letter in text)
            {
                node = node.Children[letter - EnglishLowercase.FirstLetter];

                if (node is null)
                {
                    return null;
                }
            }

            return node;
        }

        private sealed class Node
        {
            public Node?[] Children { get; } = new Node?[EnglishLetters.Size];

            public bool IsWord { get; set; }
        }
    }
}
