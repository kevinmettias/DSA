using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Language.English;
using DenseWords = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PrefixSet<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.DenseTrieLayout,
        bool>>;
using HashedWords = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PrefixSet<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.HashedChildren<bool>>;
using PackedWords = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PrefixSet<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
        bool>>;

namespace DSAExperimentation.LeetCode.Solutions.ImplementTrie;

// LeetCode 208. Implement Trie (Prefix Tree): a design problem - insert/search/
// startsWith against a fixed word set, not a single return value.
//
// The layer LC 208 asks for is a set of words with prefix queries, IPrefixSet, its
// verbs traded for the contract's (Add, HasKey, HasPrefix) as LRUCacheSolution trades
// get and put for ICache's. The textbook strategy is the node chain LC 208's own
// editorial writes - each node 26 child slots and an end-of-word mark, over plain
// arrays - and it implements IPrefixSet itself, so one contract suite (PrefixSetTests)
// holds it and the one trie to the same answers. The one trie is a strategy under
// each store LC 208's lowercase words allow, so the benchmark shows where the
// hand-written chain wins and by how much (ARCHITECTURE 17.5).
[LeetCodeQuestion("implement-trie-prefix-tree", LeetCodeAnswerOrder.AsWritten, Calls = typeof(ImplementTrieCalls))]
internal static class ImplementTrieSolution
{
    // The textbook answer, the arm the one trie is measured against (ARCHITECTURE 17.5).
    public static ArrayNodeTrie CreateByArrayNodes() => new();

    public static PackedWords CreateByPackedSlots() => new(new());

    public static DenseWords CreateByDenseSlots() => new(new());

    public static HashedWords CreateByHashedChildren() => new(new());

    internal sealed class ArrayNodeTrie : IPrefixSet
    {
        private readonly Node _root = new();

        public int Count { get; private set; }

        public void Add(string key)
        {
            var node = _root;

            foreach (var letter in key)
            {
                node = node.Children[letter - EnglishLowercase.FirstLetter] ??= new Node();
            }

            if (!node.IsWord)
            {
                node.IsWord = true;
                Count++;
            }
        }

        public bool HasKey(string key)
        {
            var node = NodeAt(key);

            return node is not null && node.IsWord;
        }

        // Every key starts with the empty prefix, so it asks whether any key was added (IPrefixSet).
        public bool HasPrefix(string prefix) => prefix.Length == 0 ? Count > 0 : NodeAt(prefix) is not null;

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
