using DSAExperimentation.Algorithms.Folding;
using SumNode = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieNode<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
        int>,
    int>;

namespace DSAExperimentation.LeetCode.Solutions.MapSumPairs;

// Folds an alphabet-trie subtree into the sum of every node's own value plus
// its children's already-folded sums - a node with no value contributes 0 of its
// own, exactly SizeAlgebra's "1 + children.Sum()" shape with the constant traded
// for the node's stored value. This answers LC677's sum(prefix) and nothing else,
// which is why it lives beside the solution rather than in Algorithms/Folding.
internal readonly struct SumValuesAlgebra : IFoldAlgebra<SumNode, int>
{
    public int Empty => 0;

    public int Combine(SumNode node, ReadOnlySpan<int> children)
    {
        var total = node.HasValue ? node.Value : 0;

        foreach (var childTotal in children)
        {
            total += childTotal;
        }

        return total;
    }
}
