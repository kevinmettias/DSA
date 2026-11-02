using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MinimumEdgeTogglesOnATree;

// Materializes LC 3812's undirected edges[] as a RootedTreeNode tree rooted at 0
// (rooted by DataStructures' EdgeListTree, as ShortestPathInAWeightedTreeSolution's
// is), plus - the one thing a plain parent array does not carry -
// ParentEdgeIndex[node.Id]: the original index into edges[] of the edge connecting
// that node to its parent, which is what the answer has to report. A witness
// meaningful only to this problem, so it lives here rather than at the tier's root
// (§17.3).
internal sealed record ToggleTree(RootedTreeNode Root, int[] ParentEdgeIndex)
{
    public static ToggleTree Build(int nodeCount, int[][] edges)
    {
        var (parent, parentEdgeIndex) = EdgeListTree.ParentsFrom(nodeCount, edges, root: 0, firstId: 0);

        var nodes = ParentArrayTree.Build(parent);
        return new ToggleTree(nodes[0], parentEdgeIndex);
    }
}
