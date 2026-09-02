using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinimizeMaximumComponentCost;

// The IEdgeTopology witness ComponentNode needs to be handed to
// Algorithms.MinimumSpanningTrees.MinimumSpanningTree.Kruskal - the same role
// WeightedGridTopology plays for WeightedGridNode, just for an arbitrary
// (non-grid) weighted graph.
internal readonly struct ComponentTopology : IEdgeTopology<ComponentNode, ListEdges<ComponentNode, int>, int>
{
    public static ListEdges<ComponentNode, int> GetEdges(ComponentNode node) => new(node.Edges);
}
