using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTree;

// LeetCode 1489. Find Critical and Pseudo-Critical Edges in Minimum Spanning Tree: an
// edge is critical when dropping it makes every spanning tree heavier (or disconnects
// the graph outright), and pseudo-critical when some minimum spanning tree contains it
// even though no MST is forced to.
//
// Both strategies take that definition literally and run Kruskal's greedy scan three
// ways per edge - once for the baseline MST weight, once with the edge excluded, once
// with the edge forced in first - so they differ in exactly one thing: how "are these
// two endpoints already in the same component?" is answered.
//
// ClassifyEdgesByBfsConnectivity is the textbook answer with no union-find at all: it
// keeps the edges accepted so far as an adjacency list and re-runs a fresh BFS for
// every connectivity question, O(V+E) each time. ClassifyEdgesByDisjointSet hands the
// same question to this repo's own DataStructures.DisjointSet.DisjointSet - the
// primitive Algorithms.MinimumSpanningTrees.MinimumSpanningTree.Kruskal composes
// internally - whose path compression and union-by-rank answer it in O(a(n))
// amortized.
//
// MinimumSpanningTree.Kruskal itself is not reused directly because its
// IEdgeTopology-based edge discovery has no notion of "skip edge #i" or "force edge #i
// in", and both probes need per-edge index identity, since weights tie.
internal static class FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeSolution
{
    // "No edge is excluded" / "no edge is forced" for an unconstrained probe.
    private const int NoEdge = -1;

    // The textbook answer: no union-find, just an adjacency list of the edges accepted
    // so far and a fresh BFS per connectivity question. Deliberately written without
    // this repo's primitives - it is the arm the composed strategy below has to justify
    // itself against.
    public static (int[] Critical, int[] PseudoCritical) ClassifyEdgesByBfsConnectivity(
        int nodeCount, int[][] edges)
    {
        var graph = WeightedEdgeList.Build(nodeCount, edges);

        return ClassifyEdgesByBfsConnectivity(graph);
    }

    public static (int[] Critical, int[] PseudoCritical) ClassifyEdgesByBfsConnectivity(WeightedEdgeList graph) =>
        Classify(graph, BfsSpanningWeight);

    // The same three-probe classification over this repo's DisjointSet.
    public static (int[] Critical, int[] PseudoCritical) ClassifyEdgesByDisjointSet(
        int nodeCount, int[][] edges)
    {
        var graph = WeightedEdgeList.Build(nodeCount, edges);

        return ClassifyEdgesByDisjointSet(graph);
    }

    public static (int[] Critical, int[] PseudoCritical) ClassifyEdgesByDisjointSet(WeightedEdgeList graph) =>
        Classify(graph, DisjointSetSpanningWeight);

    private static (int[] Critical, int[] PseudoCritical) Classify(
        WeightedEdgeList graph, SpanningWeight spanningWeight)
    {
        var baseline = spanningWeight(graph, EdgeProbe.Unconstrained)
            ?? throw new InvalidOperationException("LeetCode 1489 guarantees a connected input graph.");

        var critical = new List<int>();
        var pseudoCritical = new List<int>();

        for (var index = 0; index < graph.Edges.Length; index++)
        {
            if (IsCritical(graph, spanningWeight, index, baseline))
            {
                critical.Add(index);
            }
            else if (spanningWeight(graph, EdgeProbe.Forcing(index)) == baseline)
            {
                pseudoCritical.Add(index);
            }
        }

        return ([.. critical], [.. pseudoCritical]);
    }

    // Dropping a critical edge either leaves the graph disconnected (no spanning weight
    // at all) or forces a heavier tree.
    private static bool IsCritical(
        WeightedEdgeList graph, SpanningWeight spanningWeight, int index, int baseline)
    {
        var withoutEdge = spanningWeight(graph, EdgeProbe.Skipping(index));

        return withoutEdge is null || withoutEdge > baseline;
    }

    // Weight of the minimum spanning tree under one probe's constraint, or null when
    // the constrained graph has no spanning tree at all.
    private delegate int? SpanningWeight(WeightedEdgeList graph, EdgeProbe probe);

    private static int? DisjointSetSpanningWeight(WeightedEdgeList graph, EdgeProbe probe)
    {
        var components = new DisjointSet(graph.NodeCount);
        var tally = new SpanningTally();

        if (probe.ForceIndex != NoEdge)
        {
            UnionEdge(components, graph.Edges[probe.ForceIndex], tally);
        }

        foreach (var index in graph.ByWeight)
        {
            TryUnionEdge(graph, components, index, probe, tally);
        }

        return tally.SpanningWeightOf(graph.NodeCount);
    }

    private static void TryUnionEdge(
        WeightedEdgeList graph, DisjointSet components, int index, EdgeProbe probe, SpanningTally tally)
    {
        if (probe.Excludes(index))
        {
            return;
        }

        var edge = graph.Edges[index];

        if (!components.IsConnected(edge[0], edge[1]))
        {
            UnionEdge(components, edge, tally);
        }
    }

    private static void UnionEdge(DisjointSet components, int[] edge, SpanningTally tally)
    {
        components.Union(edge[0], edge[1]);
        tally.Accept(WeightedEdgeList.WeightOf(edge));
    }

    private static int? BfsSpanningWeight(WeightedEdgeList graph, EdgeProbe probe)
    {
        var adjacency = new List<int>[graph.NodeCount];

        for (var node = 0; node < graph.NodeCount; node++)
        {
            adjacency[node] = [];
        }

        var tally = new SpanningTally();

        if (probe.ForceIndex != NoEdge)
        {
            AcceptBfsEdge(adjacency, graph.Edges[probe.ForceIndex], tally);
        }

        foreach (var index in graph.ByWeight)
        {
            TryAcceptBfsEdge(graph, adjacency, index, probe, tally);
        }

        return tally.SpanningWeightOf(graph.NodeCount);
    }

    private static void TryAcceptBfsEdge(
        WeightedEdgeList graph, List<int>[] adjacency, int index, EdgeProbe probe, SpanningTally tally)
    {
        if (probe.Excludes(index))
        {
            return;
        }

        var edge = graph.Edges[index];

        if (!ReachableViaBfs(adjacency, edge[0], edge[1]))
        {
            AcceptBfsEdge(adjacency, edge, tally);
        }
    }

    private static void AcceptBfsEdge(List<int>[] adjacency, int[] edge, SpanningTally tally)
    {
        adjacency[edge[0]].Add(edge[1]);
        adjacency[edge[1]].Add(edge[0]);
        tally.Accept(WeightedEdgeList.WeightOf(edge));
    }

    private static bool ReachableViaBfs(List<int>[] adjacency, int start, int target)
    {
        if (start == target)
        {
            return true;
        }

        var visited = new bool[adjacency.Length];
        var queue = new Queue<int>();
        visited[start] = true;
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            if (VisitNeighbors(adjacency, queue, visited, target))
            {
                return true;
            }
        }

        return false;
    }

    private static bool VisitNeighbors(List<int>[] adjacency, Queue<int> queue, bool[] visited, int target)
    {
        foreach (var neighbor in adjacency[queue.Dequeue()])
        {
            if (neighbor == target)
            {
                return true;
            }

            if (!visited[neighbor])
            {
                visited[neighbor] = true;
                queue.Enqueue(neighbor);
            }
        }

        return false;
    }

    // One classification probe: the edge index Kruskal's scan must ignore (the
    // excluded-edge question) and the edge index it accepts before the scan starts
    // (the forced-edge question). Either may be NoEdge.
    private readonly record struct EdgeProbe(int SkipIndex, int ForceIndex)
    {
        public static EdgeProbe Unconstrained => new(NoEdge, NoEdge);

        public static EdgeProbe Skipping(int index) => new(index, NoEdge);

        // A forced edge is also skipped by the scan, since it is already accepted.
        public static EdgeProbe Forcing(int index) => new(NoEdge, index);

        public bool Excludes(int index) => index == SkipIndex || index == ForceIndex;
    }

    // Running weight and edge count of the tree built so far, shared by both strategies'
    // accept helpers - a class rather than a struct so those helpers can update the
    // caller's totals without ref parameters.
    private sealed class SpanningTally
    {
        private int _totalWeight;
        private int _edgesUsed;

        public void Accept(int weight)
        {
            _totalWeight += weight;
            _edgesUsed++;
        }

        // A spanning tree over n nodes has exactly n - 1 edges; anything less means the
        // constrained graph was disconnected.
        public int? SpanningWeightOf(int nodeCount) => _edgesUsed == nodeCount - 1 ? _totalWeight : null;
    }
}
