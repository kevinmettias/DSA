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

        var parent = new int[n];
        var parentEdgeIndex = new int[n];
        Array.Fill(parent, NoParentAssignedYet);
        parent[0] = -1;

        var queue = new RepoQueue();
        queue.Enqueue(0);

        while (queue.TryDequeue(out var current))
        {
            foreach (var (next, edgeIndex) in adjacency[current])
            {
                if (parent[next] != NoParentAssignedYet)
                {
                    continue;
                }

                parent[next] = current;
                parentEdgeIndex[next] = edgeIndex;
                queue.Enqueue(next);
            }
        }

        var nodes = ParentArrayTree.Build(parent);
        return new ToggleTree(nodes[0], parentEdgeIndex);
    }
}
