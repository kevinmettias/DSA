using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveMaxNumberOfEdgesToKeepGraphFullyTraversable;

// LeetCode 1579. Remove Max Number of Edges to Keep Graph Fully Traversable: two of
// this repo's own DisjointSet instances (NumberOfOperationsToMakeNetworkConnectedTests
// precedent), one per traverser. Type-3 (both) edges are unioned into both DSUs first -
// greedily preferring shared edges is what a correct max-removal count requires, since
// a shared edge can never be more expensive to keep than the two single-owner edges it
// could otherwise be replaced by. Type-1/Type-2 edges then only ever touch their own
// DSU. An edge is "used" (kept) only when IsConnected reports its endpoints weren't
// already joined - every edge that fails that check is one of the ones being removed.
// -1 whenever either DSU still has more than one component once every edge has been
// offered to it. Nodes are converted from LeetCode's 1-indexed labels to this repo's
// dense 0-indexed DisjointSet ids at every use.
public sealed partial class RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableTests
{
    [Fact]
    public void MaxNumEdgesToRemove_BothTraversableWithRedundantEdges_ReturnsTwo()
    {
        int[][] edges =
        [
            [3, 1, 2], [3, 2, 3], [1, 1, 3], [1, 2, 4], [1, 1, 2], [2, 3, 4],
        ];

        var removedEdgeCount = MaxNumEdgesToRemove(4, edges);
        Assert.Equal(2, removedEdgeCount);
    }

    [Fact]
    public void MaxNumEdgesToRemove_NoRedundantEdges_ReturnsZero()
    {
        int[][] edges = [[3, 1, 2], [3, 2, 3], [1, 1, 4], [2, 1, 4]];

        var removedEdgeCount = MaxNumEdgesToRemove(4, edges);
        Assert.Equal(0, removedEdgeCount);
    }

    [Fact]
    public void MaxNumEdgesToRemove_BobCannotFullyTraverse_ReturnsNegativeOne()
    {
        int[][] edges = [[3, 2, 3], [1, 1, 2], [2, 3, 4]];

        var removedEdgeCount = MaxNumEdgesToRemove(4, edges);
        Assert.Equal(-1, removedEdgeCount);
    }

    private static int MaxNumEdgesToRemove(int n, int[][] edges)
    {
        var alice = new DisjointSet(n);
        var bob = new DisjointSet(n);
        var usedEdges = UnionSharedEdges(alice, bob, edges);

        usedEdges += UnionOwnEdges(alice, edges, ownerType: 1);
        usedEdges += UnionOwnEdges(bob, edges, ownerType: 2);

        return IsFullyConnected(alice, n) && IsFullyConnected(bob, n)
            ? edges.Length - usedEdges
            : -1;
    }

    // Type-3 (both) edges are unioned into both DSUs together first - greedily
    // preferring shared edges is what a correct max-removal count requires.
    private static int UnionSharedEdges(DisjointSet alice, DisjointSet bob, int[][] edges)
    {
        var used = 0;

        foreach (var edge in edges)
        {
            if (edge[0] != 3)
            {
                continue;
            }

            var (u, v) = (edge[1] - 1, edge[2] - 1);
            if (alice.IsConnected(u, v))
            {
                continue;
            }

            alice.Union(u, v);
            bob.Union(u, v);
            used++;
        }

        return used;
    }

    private static int UnionOwnEdges(DisjointSet components, int[][] edges, int ownerType)
    {
        var used = 0;

        foreach (var edge in edges)
        {
            if (edge[0] != ownerType)
            {
                continue;
            }

            var (u, v) = (edge[1] - 1, edge[2] - 1);
            if (components.IsConnected(u, v))
            {
                continue;
            }

            components.Union(u, v);
            used++;
        }

        return used;
    }

    private static bool IsFullyConnected(DisjointSet components, int n)
    {
        var root = components.Find(0);

        for (var i = 1; i < n; i++)
        {
            if (components.Find(i) != root)
            {
                return false;
            }
        }

        return true;
    }
}
