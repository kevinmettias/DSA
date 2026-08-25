using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.Graph.Contracts.Ordering;

public sealed partial class TrieTests
{
    [Fact]
    public void Fold_CountsWordsInTrie()
    {
        // "car" is a prefix of "card" - both must count as separate words, and the
        // shared "car" node must be marked IsWord independently of "card" continuing
        // past it.
        var root = TrieTrees.FromWords("cat", "car", "card", "dog");

        var count = TreeFold.Fold<
            TrieNode, TrieTopology, SparseArrayChildren<TrieNode>,
            NaturalChildOrder<TrieNode, SparseArrayChildren<TrieNode>>, SparseArrayChildren<TrieNode>,
            WordCountFoldAlgebra, int>(root);

        Assert.Equal(4, count);
    }

    [Fact]
    public void Fold_EmptyTrie_CountsZeroWords()
    {
        var root = TrieTrees.FromWords();

        var count = TreeFold.Fold<
            TrieNode, TrieTopology, SparseArrayChildren<TrieNode>,
            NaturalChildOrder<TrieNode, SparseArrayChildren<TrieNode>>, SparseArrayChildren<TrieNode>,
            WordCountFoldAlgebra, int>(root);

        Assert.Equal(0, count);
    }
}
