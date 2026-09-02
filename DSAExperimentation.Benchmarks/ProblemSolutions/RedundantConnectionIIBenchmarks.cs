using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Redundant Connection II (LC 685): the textbook brute force retries validity from
// scratch for every candidate edge removal (an in-degree pass plus an uncompressed
// find-root cycle pass per candidate, O(n) each, O(n^2) overall) against this repo's
// own DisjointSet-based approach, which finds the in-degree-2 node's two candidate
// edges up front and settles the answer in at most two O(n * alpha(n)) DisjointSet
// passes total.
[MemoryDiagnoser]
public class RedundantConnectionIIBenchmarks
{
    // The chain's extra cross edge targets node 2, which is what gives it two parents.
    private const int DoublyParentedNode = 2;

    [Params(200, 5_000)]
    public int NodeCount;

    private int[][] _edges = null!;

    [GlobalSetup]
    public void Setup()
    {
        // A straight-line chain 1->2->...->n plus one extra edge n->2, giving node 2
        // two parents - edges targeting node 2 sit at the very first and very last
        // array positions, so every candidate removal in between needs a near-full
        // scan before either check can rule it out. Removing the extra (last) edge
        // is the only fix, forcing both strategies through real work rather than an
        // early-exit on the first candidate tried.
        var edges = new int[NodeCount][];
        for (var i = 0; i < NodeCount - 1; i++)
        {
            var parent = i + 1;
            edges[i] = [parent, parent + 1];
        }

        edges[NodeCount - 1] = [NodeCount, DoublyParentedNode];
        _edges = edges;
    }

    [Benchmark(Baseline = true)]
    public int[] NaiveRemovalScan() => FindByRemovalScan(_edges);

    [Benchmark]
    public int[] DisjointSetTwoPass() => FindByDisjointSet(_edges);

    private static int[] FindByRemovalScan(int[][] edges)
    {
        var n = edges.Length;

        for (var skip = 0; skip < n; skip++)
        {
            if (IsValidTreeWithout(edges, n, skip))
            {
                return edges[skip];
            }
        }

        return [];
    }

    // A candidate removal is valid only if every node keeps at most one parent AND
    // the remaining edges are acyclic - checked as two separate O(n) passes, neither
    // one alone (as LC684's own single-cycle-check would be) sufficient for the
    // directed, two-failure-mode shape LC685 adds over LC684.
    private static bool IsValidTreeWithout(int[][] edges, int n, int skip)
    {
        if (!HasAtMostOneParentEach(edges, n, skip))
        {
            return false;
        }

        return IsAcyclicWithoutCompression(edges, n, skip);
    }

    private static bool HasAtMostOneParentEach(int[][] edges, int n, int skip)
    {
        var inDegree = new int[n + 1];

        for (var i = 0; i < n; i++)
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

    private static bool IsAcyclicWithoutCompression(int[][] edges, int n, int skip)
    {
        var parent = new int[n + 1];
        for (var i = 0; i <= n; i++)
        {
            parent[i] = i;
        }

        for (var i = 0; i < n; i++)
        {
            if (UnionEdgeDetectsCycle(edges, parent, skip, i))
            {
                return false;
            }
        }

        return true;
    }

    private static bool UnionEdgeDetectsCycle(int[][] edges, int[] parent, int skip, int i)
    {
        if (i == skip)
        {
            return false;
        }

        var (u, v) = (edges[i][0], edges[i][1]);
        var rootU = FindRootNoCompression(parent, u);
        var rootV = FindRootNoCompression(parent, v);

        if (rootU == rootV)
        {
            return true;
        }

        parent[rootV] = rootU;
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

    private static int[] FindByDisjointSet(int[][] edges)
    {
        var n = edges.Length;
        var (conflictEdge, priorEdge) = FindTwoParentConflict(edges, n);

        if (conflictEdge == -1)
        {
            return FindCycleEdge(edges, n, skip: -1);
        }

        return ResolveConflict(edges, n, conflictEdge, priorEdge);
    }

    private static (int ConflictEdge, int PriorEdge) FindTwoParentConflict(int[][] edges, int n)
    {
        var parentEdgeOf = new int[n + 1];
        Array.Fill(parentEdgeOf, -1);

        var conflictEdge = -1;
        var priorEdge = -1;

        for (var i = 0; i < n; i++)
        {
            var child = edges[i][1];
            if (parentEdgeOf[child] != -1)
            {
                priorEdge = parentEdgeOf[child];
                conflictEdge = i;
                break;
            }

            parentEdgeOf[child] = i;
        }

        return (conflictEdge, priorEdge);
    }

    private static int[] ResolveConflict(int[][] edges, int n, int conflictEdge, int priorEdge)
    {
        var cycleEdge = FindCycleEdge(edges, n, skip: conflictEdge);
        return cycleEdge.Length == 0 ? edges[conflictEdge] : edges[priorEdge];
    }

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
