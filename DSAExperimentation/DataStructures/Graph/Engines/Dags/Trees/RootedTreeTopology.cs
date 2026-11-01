using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// ITreeTopology, not merely IDagTopology: a parent array gives every node exactly
// one parent, so unique ancestry holds by construction and a fold needs no
// revisit-tracking.
/// <summary>
/// The tree topology witness for <see cref="RootedTreeNode"/>, whose children are the ones its child list holds.
/// </summary>
public readonly struct RootedTreeTopology : ITreeTopology<RootedTreeNode, ListChildren<RootedTreeNode>>
{
    /// <summary>Lists a node's children, in the order its child list holds them.</summary>
    /// <param name="node">The node whose children to list.</param>
    /// <returns>A view over the node's own child list, not a copy of it.</returns>
    public static ListChildren<RootedTreeNode> GetChildren(RootedTreeNode node) => new(node.Children);
}
