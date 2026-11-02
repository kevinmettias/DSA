using IdQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// Roots LeetCode's edge-list encoding of a tree - a node count and n - 1 undirected edges, each
// row's first two entries its endpoints - by a breadth-first walk from one node. The result is
// the parent-array encoding ParentArrayTree reads, plus the one thing a parent array cannot
// recover once the walk has run: the index of the edge that joins each node to its parent. A
// problem reads a weight from that row, or reports the index itself as its answer.
//
// Three solutions had written this walk by hand (LC 2920, 3515 and 3812), each differing only in
// what it kept beside the parent: nothing, the edge's weight, or the edge's index. The index is
// the one that covers all three.
//
// Distinct from its neighbours. ParentArrayTree materializes a parent array that already names
// a root; PreOrderTour lays out a tree that is already rooted. Here the edges name no direction,
// and the walk is what picks one. It is iterative, so a chain of LeetCode's 10^5 nodes is safe.
/// <summary>Roots an undirected tree, given as an edge list, at one node.</summary>
public static class EdgeListTree
{
    // A node the walk has not reached yet: negative, and distinct from the root's NoParent.
    private const int Unreached = -2;
    private const string UnreachedNodesMessage = "The edges must connect every id below nodeCount to the root.";

    // firstId is what LeetCode numbers its first node with, 0 or 1, and comes off every endpoint,
    // so the ids returned run 0..nodeCount-1 whichever numbering the edges use.
    /// <summary>
    /// Walks an undirected tree breadth-first from <paramref name="root"/>, giving each node its parent and the
    /// edge that joins them.
    /// </summary>
    /// <param name="nodeCount">How many nodes the tree has.</param>
    /// <param name="edges">The tree's edges; each row's first two entries are its endpoints, and any further entries are ignored.</param>
    /// <param name="root">The node to hang the tree from, numbered from 0.</param>
    /// <param name="firstId">The id the edges number their first node with, subtracted from every endpoint.</param>
    /// <returns>
    /// Each node's parent and the index into <paramref name="edges"/> of the edge joining them, both numbered from
    /// 0 and both -1 for the root.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when the edges leave a node unconnected to the root.</exception>
    public static (int[] Parent, int[] ParentEdge) ParentsFrom(int nodeCount, int[][] edges, int root, int firstId)
    {
        var neighbours = NeighboursOf(nodeCount, edges, firstId);
        var parent = new int[nodeCount];
        var parentEdge = new int[nodeCount];
        Array.Fill(parent, Unreached);
        parent[root] = ParentArrayTree.NoParent;
        parentEdge[root] = ParentArrayTree.NoParent;

        WalkFrom(root, neighbours, parent, parentEdge);

        if (Array.IndexOf(parent, Unreached) >= 0)
        {
            throw new ArgumentException(UnreachedNodesMessage, nameof(edges));
        }

        return (parent, parentEdge);
    }

    // Both directions of every edge, each side carrying the index the edge came from.
    private static List<(int Next, int Edge)>[] NeighboursOf(int nodeCount, int[][] edges, int firstId)
    {
        var neighbours = new List<(int Next, int Edge)>[nodeCount];

        for (var id = 0; id < nodeCount; id++)
        {
            neighbours[id] = [];
        }

        for (var edge = 0; edge < edges.Length; edge++)
        {
            var (u, v) = (edges[edge][0] - firstId, edges[edge][1] - firstId);
            neighbours[u].Add((v, edge));
            neighbours[v].Add((u, edge));
        }

        return neighbours;
    }

    // First visit wins: a neighbour that already has a parent was reached earlier, and on a tree
    // the edge to it is the edge back up. So every node is enqueued once.
    private static void WalkFrom(int root, List<(int Next, int Edge)>[] neighbours, int[] parent, int[] parentEdge)
    {
        var pending = new IdQueue();
        pending.Enqueue(root);

        while (pending.TryDequeue(out var current))
        {
            foreach (var (next, edge) in neighbours[current])
            {
                if (parent[next] != Unreached)
                {
                    continue;
                }

                parent[next] = current;
                parentEdge[next] = edge;
                pending.Enqueue(next);
            }
        }
    }
}
