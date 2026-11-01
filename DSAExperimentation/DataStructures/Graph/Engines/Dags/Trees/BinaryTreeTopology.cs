using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// Mirrors GridTopology exactly: a one-line witness over a node type that already
// carries everything GetChildren needs. ITreeTopology (unique ancestry, no sharing)
// is the correct tier, not the weaker IGraphTopology/IDagTopology - a binary tree
// built from Left/Right assignments has no way to express a shared or cyclic node.
/// <summary>
/// The tree topology over <see cref="BinaryTreeNode{TValue}"/>, giving a node's non-null Left and Right.
/// </summary>
public readonly struct BinaryTreeTopology<TValue> : ITreeTopology<BinaryTreeNode<TValue>, BinaryTreeChildren<TValue>>
{
    /// <summary>Returns a node's present children, left before right.</summary>
    /// <param name="node">The node whose children to return.</param>
    /// <returns>A view of the node's non-null Left and Right, in that order.</returns>
    public static BinaryTreeChildren<TValue> GetChildren(BinaryTreeNode<TValue> node) => new(node);
}
