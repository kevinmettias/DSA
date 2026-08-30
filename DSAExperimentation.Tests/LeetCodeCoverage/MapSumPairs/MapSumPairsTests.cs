using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MapSumPairs;

// LeetCode 677. Map Sum Pairs: this repo's own LowercaseTrie<int> (bounded-alphabet
// Trie, a real ITreeTopology witness per ARCHITECTURE.md §13.8) stores each key's
// value at its end-of-word node - insert reuses Set's own overwrite-on-existing-key
// semantics directly, exactly LC677's own "override to the new pair" rule. sum(prefix)
// walks to the prefix's node (via the trie's already-public Root/Children, the same
// navigation LowercaseTrieTests.WalkTo already rehearses) and folds a
// SumValuesAlgebra - Combine = this node's own value plus its children's already-
// folded sums, the same shape SizeAlgebra.Combine = 1 + children.Sum() already uses
// for TreeMetrics.Size - over its subtree via TreeFold.Fold, with no prefix-sum-
// specific trie code needed. A missing prefix's WalkTo returns null, and
// TreeFold.Fold(null) already resolves to TAlgebra.Empty = 0, so "no key shares this
// prefix" needs no special-casing either.
public sealed class MapSumPairsTests
{
    [Fact]
    public void Sum_LeetCodeExample_ReturnsExpectedTotals()
    {
        var mapSum = new MapSum();

        mapSum.Insert("apple", 3);
        Assert.Equal(3, mapSum.Sum("ap"));

        mapSum.Insert("app", 2);
        Assert.Equal(5, mapSum.Sum("ap"));
    }

    [Fact]
    public void Insert_ExistingKey_OverridesPreviousValue()
    {
        var mapSum = new MapSum();

        mapSum.Insert("apple", 3);
        mapSum.Insert("apple", 10);

        Assert.Equal(10, mapSum.Sum("apple"));
    }

    [Fact]
    public void Sum_UnknownPrefix_ReturnsZero()
    {
        var mapSum = new MapSum();
        mapSum.Insert("apple", 3);

        Assert.Equal(0, mapSum.Sum("banana"));
    }

    [Fact]
    public void Sum_EmptyPrefix_SumsEveryInsertedValue()
    {
        var mapSum = new MapSum();
        mapSum.Insert("apple", 3);
        mapSum.Insert("app", 2);
        mapSum.Insert("banana", 4);

        Assert.Equal(9, mapSum.Sum(""));
    }

    private sealed class MapSum
    {
        private readonly LowercaseTrie<int> _trie = new();

        public void Insert(string key, int val) => _trie.Set(key, val);

        public int Sum(string prefix) => TreeFold.Fold<
            LowercaseTrieNode<int>, LowercaseTrieTopology<int>, SparseArrayChildren<LowercaseTrieNode<int>>,
            NaturalChildOrder<LowercaseTrieNode<int>, SparseArrayChildren<LowercaseTrieNode<int>>>,
            SparseArrayChildren<LowercaseTrieNode<int>>, SumValuesAlgebra, int>(WalkTo(_trie.Root, prefix));

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

    private readonly struct SumValuesAlgebra : IFoldAlgebra<LowercaseTrieNode<int>, int>
    {
        public static int Empty => 0;

        public static int Combine(LowercaseTrieNode<int> node, IReadOnlyList<int> children)
            => (node.HasValue ? node.Value : 0) + children.Sum();
    }
}
