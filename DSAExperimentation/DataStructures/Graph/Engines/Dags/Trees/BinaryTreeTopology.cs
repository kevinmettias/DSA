using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// Mirrors GridTopology exactly: a one-line witness over a node type that already
// carries everything GetChildren needs. ITreeTopology (unique ancestry, no sharing)
// is the correct tier, not the weaker IGraphTopology/IDagTopology - a binary tree
// built from Left/Right assignments has no way to express a shared or cyclic node.
internal readonly struct BinaryTreeTopology<TValue> : ITreeTopology<BinaryTreeNode<TValue>, BinaryTreeChildren<TValue>>
{
    public static BinaryTreeChildren<TValue> GetChildren(BinaryTreeNode<TValue> node) => new(node);
}
