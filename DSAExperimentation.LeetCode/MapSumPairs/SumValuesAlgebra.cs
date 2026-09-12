using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MapSumPairs;

// Folds a LowercaseTrie<int> subtree into the sum of every node's own value plus
// its children's already-folded sums - a node with no value contributes 0 of its
// own, exactly SizeAlgebra's "1 + children.Sum()" shape with the constant traded
// for the node's stored value. This answers LC677's sum(prefix) and nothing else,
// which is why it lives beside the solution rather than in Algorithms/Folding.
internal readonly struct SumValuesAlgebra : IFoldAlgebra<LowercaseTrieNode<int>, int>
{
    public static int Empty => 0;

    public static int Combine(LowercaseTrieNode<int> node, IReadOnlyList<int> children)
        => (node.HasValue ? node.Value : 0) + children.Sum();
}
