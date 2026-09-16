using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.RedundantConnectionII;

// LeetCode 685. Redundant Connection II: the directed-graph sibling of LC 684
// (Redundant Connection). A rooted tree plus one extra directed edge fails one of
// two ways - a node ends up with two parents, or (with every node still having at
// most one parent) the edges close a cycle - and DisjointSet.IsConnected/Union
// alone only detects the second.
internal static class RedundantConnectionIISolution
{
    // The textbook brute force: retry validity from scratch for every candidate
    // edge removal - an in-degree pass plus an uncompressed find-root cycle pass
    // per candidate, O(n) each, O(n^2) overall. Deliberately written without this
    // repo's primitives (a plain array standing in for union-find) - it is the
    // baseline the DisjointSet strategy below has to justify itself against.
    //
    // LC 685 requires the removal that occurs LAST in the input when more than one
    // candidate would work (example: a plain cycle with no two-parent conflict
    // admits a valid tree after removing any one of its cycle edges, and the
    // published answer is always the last of those). The scan therefore checks
    // every candidate and keeps the last valid one rather than returning on the
    // first hit.
    public static int[] FindRedundantEdgeByRemovalScan(int[][] edges)
    {
        var n = edges.Length;
        var lastValidRemoval = Array.Empty<int>();

        for (var skip = 0; skip < n; skip++)
        {
            if (IsValidTreeWithout(edges, n, skip))
            {
                lastValidRemoval = edges[skip];
            }
        }

        return lastValidRemoval;
    }

    // A candidate removal is valid only if every node keeps at most one parent AND
    // the remaining edges are acyclic - checked as two separate O(n) passes, neither
    // one alone (as LC 684's own single-cycle-check would be) sufficient for the
    // directed, two-failure-mode shape LC 685 adds over LC 684.
    private static bool IsValidTreeWithout(int[][] edges, int edgeCount, int skip)
    {
        if (!HasAtMostOneParentEach(edges, edgeCount, skip))
        {
            return false;
        }

        return IsAcyclicWithoutCompression(edges, edgeCount, skip);
    }

    private static bool HasAtMostOneParentEach(int[][] edges, int edgeCount, int skip)
    {
        var inDegree = new int[edgeCount + 1];

        for (var i = 0; i < edgeCount; i++)
        {
            if (i == skip)
            {
                continue;
            }

            var child = edges[i][1];
            inDegree[child]++;

            if (inDegree[child] > 1)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsAcyclicWithoutCompression(int[][] edges, int edgeCount, int skip)
    {
        var parent = new int[edgeCount + 1];
        for (var i = 0; i <= edgeCount; i++)
        {
            parent[i] = i;
        }

        for (var i = 0; i < edgeCount; i++)
        {
            if (IsEdgeClosingCycle(edges, parent, skip, i))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsEdgeClosingCycle(int[][] edges, int[] parent, int skip, int edgeIndex)
    {
        if (edgeIndex == skip)
        {
            return false;
        }

        var (parentNode, childNode) = (edges[edgeIndex][0], edges[edgeIndex][1]);
        return IsClosingCycleOnUnion(parent, parentNode, childNode);
    }

    // Endpoints already sharing a root are what closes a cycle; otherwise the edge
    // joins the two components and closes nothing.
    private static bool IsClosingCycleOnUnion(int[] parent, int firstEndpoint, int secondEndpoint)
    {
        var firstRoot = FindRootNoCompression(parent, firstEndpoint);
        var secondRoot = FindRootNoCompression(parent, secondEndpoint);

        if (firstRoot == secondRoot)
        {
            return true;
        }

        parent[secondRoot] = firstRoot;
        return false;
    }

    private static int FindRootNoCompression(int[] parent, int id)
    {
        while (parent[id] != id)
        {
            id = parent[id];
        }

        return id;
    }

    // This repo's own approach: find the in-degree-2 node's two candidate edges up
    // front, then reuse DisjointSet's cycle scan to decide which of the two is safe
    // to drop.
    public static int[] FindRedundantEdgeByDisjointSet(int[][] edges)
    {
        var n = edges.Length;
        var (conflictEdge, priorEdge) = FindConflictingParentEdge(edges);

        return conflictEdge == -1
            ? FindCycleEdge(edges, n, skip: -1)
            : ResolveTwoParentConflict(edges, conflictEdge, priorEdge);
    }

    // Walks every edge once, looking for the first child that already has a parent
    // edge - that pair is the "two parents" conflict a plain DisjointSet cycle scan
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
        return cycleEdge.Length == 0 ? EdgeAt(edges, conflictEdge) : EdgeAt(edges, priorEdge);
    }

    // edges is the problem's own [parent, child] row per edge, so this is the edge
    // standing at that row.
    private static int[] EdgeAt(int[][] edges, int index) => edges[index];

    // Builds the DisjointSet over every edge except `skip` and returns the first edge
    // that closes a cycle, or [] if none does.
    private static int[] FindCycleEdge(int[][] edges, int edgeCount, int skip)
    {
        var components = new DisjointSet(edgeCount + 1);

        for (var i = 0; i < edgeCount; i++)
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
