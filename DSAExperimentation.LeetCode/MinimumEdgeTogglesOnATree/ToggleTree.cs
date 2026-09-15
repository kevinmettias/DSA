using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.MinimumEdgeTogglesOnATree;

// Materializes LC 3812's undirected edges[] as a RootedTreeNode tree rooted at 0
// (the same BFS-to-parent-array conversion ShortestPathInAWeightedTreeSolution
// uses, via this repo's own Queue<int>), plus - the one thing a plain parent array
// does not carry - ParentEdgeIndex[node.Id]: the original index into edges[] of
// the edge connecting that node to its parent, which is what the answer has to
// report. A witness meaningful only to this problem, so it lives here rather than
// in Domain/ (§17.3).
internal sealed record ToggleTree(RootedTreeNode Root, int[] ParentEdgeIndex)
{
    private const int NoParentAssignedYet = -2;

    public static ToggleTree Build(int n, int[][] edges)
    {
        var adjacency = BuildAdjacency(n, edges);
        var (parent, parentEdgeIndex) = BuildParentArray(adjacency, n);

        var nodes = ParentArrayTree.Build(parent);
        return new ToggleTree(nodes[0], parentEdgeIndex);
    }

    // Both directions of every edge, each side carrying the index it came from: that index
    // is the one thing a bare parent array cannot recover once the BFS has run.
    private static List<(int To, int EdgeIndex)>[] BuildAdjacency(int n, int[][] edges)
    {
        var adjacency = new List<(int To, int EdgeIndex)>[n];

        for (var i = 0; i < n; i++)
        {
            adjacency[i] = [];
        }

        for (var edgeIndex = 0; edgeIndex < edges.Length; edgeIndex++)
        {
            var (u, v) = (edges[edgeIndex][0], edges[edgeIndex][1]);
            adjacency[u].Add((v, edgeIndex));
            adjacency[v].Add((u, edgeIndex));
        }

        return adjacency;
    }

    // The BFS-to-parent-array conversion, rooted at 0, returning each node's parent and the
    // edge index that attached it.
    private static (int[] Parent, int[] ParentEdgeIndex) BuildParentArray(
        List<(int To, int EdgeIndex)>[] adjacency, int n)
    {
        var parent = new int[n];
        var parentEdgeIndex = new int[n];
        Array.Fill(parent, NoParentAssignedYet);
        parent[0] = -1;

        var queue = new RepoQueue();
        queue.Enqueue(0);

        while (queue.TryDequeue(out var current))
        {
            ClaimUnvisitedNeighbours(adjacency[current], current, (parent, parentEdgeIndex), queue);
        }

        return (parent, parentEdgeIndex);
    }

    // First visit wins: a node already carrying a parent is one the BFS reached earlier, and
    // on a tree that means the edge under consideration is the edge back to it.
    private static void ClaimUnvisitedNeighbours(
        List<(int To, int EdgeIndex)> neighbours,
        int current,
        (int[] Parent, int[] ParentEdgeIndex) assignment,
        RepoQueue queue)
    {
        foreach (var (next, edgeIndex) in neighbours)
        {
            if (assignment.Parent[next] != NoParentAssignedYet)
            {
                continue;
            }

            assignment.Parent[next] = current;
            assignment.ParentEdgeIndex[next] = edgeIndex;
            queue.Enqueue(next);
        }
    }
}
