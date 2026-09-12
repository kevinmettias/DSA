using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MapSumPairs;

// LeetCode 677. Map Sum Pairs: a design problem - insert(key, val) overrides any
// previous value for that key, and sum(prefix) totals the values of every key
// sharing that prefix. A Design problem's whole point is a sequence of mutating
// calls against one instance, so "every strategy for the problem" (ARCHITECTURE.md
// 17.3) takes the form of two classes implementing the shared IMapSumStrategy
// surface below (DesignAddAndSearchWordsDataStructureSolution precedent).
internal static class MapSumPairsSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one insert/sum script against either strategy without
    // restating it.
    internal interface IMapSumStrategy
    {
        void Insert(string key, int val);

        int Sum(string prefix);
    }

    // The textbook answer: every key/value kept in a flat BCL Dictionary (whose
    // own indexer already gives LC's "insert overrides" semantics for free), summed
    // by scanning every stored key with string.StartsWith - deliberately without
    // this repo's Trie, the arm MapSumByTrieFold has to justify itself against.
    internal sealed class MapSumByDictionaryScan : IMapSumStrategy
    {
        private readonly Dictionary<string, int> _values = [];

        public void Insert(string key, int val) => _values[key] = val;

        public int Sum(string prefix)
        {
            var total = 0;

            foreach (var (key, value) in _values)
            {
                if (key.StartsWith(prefix, StringComparison.Ordinal))
                {
                    total += value;
                }
            }

            return total;
        }
    }

    // This repo's own LowercaseTrie<int> (bounded-alphabet Trie, a real
    // ITreeTopology witness per ARCHITECTURE.md 13.8) stores each key's value at
    // its end-of-word node - insert reuses Set's own overwrite-on-existing-key
    // semantics directly, exactly LC677's own "override to the new pair" rule.
    // sum(prefix) walks to the prefix's node and folds SumValuesAlgebra - Combine =
    // this node's own value plus its children's already-folded sums, the same
    // shape SizeAlgebra.Combine = 1 + children.Sum() already uses for
    // TreeMetrics.Size - over its subtree via TreeFold.Fold, with no
    // prefix-sum-specific trie code needed. A missing prefix's WalkTo returns
    // null, and TreeFold.Fold(null) already resolves to TAlgebra.Empty = 0, so
    // "no key shares this prefix" needs no special-casing either.
    internal sealed class MapSumByTrieFold : IMapSumStrategy
    {
        private readonly LowercaseTrie<int> _trie = new();

        public void Insert(string key, int val) => _trie.Set(key, val);

        public int Sum(string prefix)
        {
            var node = WalkTo(_trie.Root, prefix);

            return TreeFold.Fold<
                LowercaseTrieNode<int>, LowercaseTrieTopology<int>, SparseArrayChildren<LowercaseTrieNode<int>>,
                NaturalChildOrder<LowercaseTrieNode<int>, SparseArrayChildren<LowercaseTrieNode<int>>>,
                SparseArrayChildren<LowercaseTrieNode<int>>, SumValuesAlgebra, int>(node);
        }

        private static LowercaseTrieNode<int>? WalkTo(LowercaseTrieNode<int> root, string prefix)
        {
            var current = root;

            foreach (var ch in prefix)
            {
                current = current.Children[ch - 'a'];

                if (current is null)
                {
                    return null;
                }
            }

            return current;
        }
    }
}
