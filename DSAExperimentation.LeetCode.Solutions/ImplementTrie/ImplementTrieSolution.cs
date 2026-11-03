using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.LeetCode.Solutions.ImplementTrie;

// LeetCode 208. Implement Trie (Prefix Tree): a design problem - insert/search/
// startsWith against a fixed word set, not a single return value.
//
// This repo's own Trie<TValue> already implements exactly this shape
// (Set/HasKey/HasPrefix, LeetCode's own verbs traded for this repo's naming - the
// same trade LRUCacheSolution makes over ICache<TKey,TValue>), so the composed
// strategy is that trie, directly. The textbook strategy is the node chain LC 208's
// own editorial writes - each node 26 child slots and an end-of-word mark, over plain
// arrays - asked HasWord and HasPrefix in place of LeetCode's search and startsWith, as
// this repo's own Trie is asked HasKey and HasPrefix.
internal static class ImplementTrieSolution
{
    // The textbook answer, the arm this repo's Trie is measured against (ARCHITECTURE 17.5).
    public static ArrayNodeTrie CreateByArrayNodes() => new();

    public static Trie<bool> CreateByTriePrimitive() => new();

    internal sealed class ArrayNodeTrie
    {
        // LC 208's words are lowercase English, one child slot per letter.
        private const int AlphabetSize = 26;
        private const char FirstLetter = 'a';

        private readonly Node _root = new();

        public void Insert(string word)
        {
            var node = _root;

            foreach (var letter in word)
            {
                node = node.Children[letter - FirstLetter] ??= new Node();
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
                node = node.Children[letter - FirstLetter];

                if (node is null)
                {
                    return null;
                }
            }

            return node;
        }

        private sealed class Node
        {
            public Node?[] Children { get; } = new Node?[AlphabetSize];

            public bool IsWord { get; set; }
        }
    }
}
