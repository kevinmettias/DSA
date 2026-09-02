using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RedundantConnectionII;

// LeetCode 685. Redundant Connection II: the directed-graph sibling of LC 684
// (Redundant Connection). A rooted tree plus one extra directed edge fails one of
// two ways - a node ends up with two parents, or (with every node still having at
// most one parent) the edges close a cycle - and DisjointSet.IsConnected/Union
// alone only detects the second. The extra step is finding the in-degree-2 node's
// two candidate edges first, then reusing the same DisjointSet cycle scan to decide
// which of the two is safe to drop.
public sealed partial class RedundantConnectionIITests
{
    [Fact]
    public void FindRedundantDirectedEdge_TwoParentsAndDroppingTheLaterEdgeWorks_ReturnsLaterEdge()
    {
        int[][] edges = [[1, 2], [1, 3], [2, 3]];

        var redundant = FindRedundantDirectedEdge(edges);

        Assert.Equal([2, 3], redundant);
    }

    [Fact]
    public void FindRedundantDirectedEdge_NoNodeHasTwoParents_ReturnsTheCycleClosingEdge()
    {
        int[][] edges = [[1, 2], [2, 3], [3, 4], [4, 1], [1, 5]];

        var redundant = FindRedundantDirectedEdge(edges);

        Assert.Equal([4, 1], redundant);
    }

    [Fact]
    public void FindRedundantDirectedEdge_TwoParentsButDroppingTheLaterEdgeStillCycles_ReturnsEarlierEdge()
    {
        int[][] edges = [[2, 1], [3, 1], [4, 2], [1, 4]];

        var redundant = FindRedundantDirectedEdge(edges);

        Assert.Equal([2, 1], redundant);
    }

    // Precondition (guaranteed by LeetCode 685's own constraints): edges describes a
    // rooted tree plus exactly one extra directed edge, so a fix is always found.
    private static int[] FindRedundantDirectedEdge(int[][] edges)
    {
        var n = edges.Length;
        var (conflictEdge, priorEdge) = FindConflictingParentEdge(edges);

        return conflictEdge == -1
            ? FindCycleEdge(edges, n, skip: -1)
            : ResolveTwoParentConflict(edges, conflictEdge, priorEdge);
    }

    // Walks every edge once, looking for the first child that already has a parent
    // edge - that pair is the "two parents" conflict this repo's DisjointSet scan
    // alone can't detect. Returns (-1, -1) when no node has two parents.
    private static (int ConflictEdge, int PriorEdge) FindConflictingParentEdge(int[][] edges)
    {
        var n = edges.Length;
        var parentEdgeOf = new int[n + 1];
        Array.Fill(parentEdgeOf, -1);

        for (var i = 0; i < n; i++)
        {
            var child = edges[i][1];
            if (parentEdgeOf[child] != -1)
            {
                return (i, parentEdgeOf[child]);
            }

            parentEdgeOf[child] = i;
        }

        return (-1, -1);
    }

    // Drops the later of the two conflicting edges first; if that still leaves a
    // cycle, the earlier one was the real redundant edge instead.
    private static int[] ResolveTwoParentConflict(int[][] edges, int conflictEdge, int priorEdge)
    {
        var cycleEdge = FindCycleEdge(edges, edges.Length, skip: conflictEdge);
        return cycleEdge.Length == 0 ? edges[conflictEdge] : edges[priorEdge];
    }

    // Builds the DisjointSet over every edge except `skip` and returns the first edge
    // that closes a cycle, or [] if none does - the same LC 684-shaped scan
    // RedundantConnectionTests.FindRedundantEdge runs, reused for both branches above.
    private static int[] FindCycleEdge(int[][] edges, int n, int skip)
    {
        var components = new DisjointSet(n + 1);

        for (var i = 0; i < n; i++)
        {
            if (i == skip)
            {
                continue;
            }

            var (parent, child) = (edges[i][0], edges[i][1]);
            if (components.IsConnected(parent, child))
            {
                return edges[i];
            }

            components.Union(parent, child);
        }

        return [];
    }
}
