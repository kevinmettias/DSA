using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// ITreeTopology, not merely IDagTopology: a parent array gives every node exactly
// one parent, so unique ancestry holds by construction and a fold needs no
// revisit-tracking.
internal readonly struct RootedTreeTopology : ITreeTopology<RootedTreeNode, ListChildren<RootedTreeNode>>
{
    public static ListChildren<RootedTreeNode> GetChildren(RootedTreeNode node) => new(node.Children);
}
