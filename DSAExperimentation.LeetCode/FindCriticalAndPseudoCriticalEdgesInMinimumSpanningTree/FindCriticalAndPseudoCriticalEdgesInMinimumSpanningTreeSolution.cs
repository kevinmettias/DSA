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

    // Both strategies are stateless, so one instance each serves every call and the
    // benchmark arms that measure these two methods allocate nothing to pick one.
    private static readonly ISpanningWeight BfsConnectivityWeight = new SpanningWeightByBfs();
    private static readonly ISpanningWeight DisjointSetWeight = new SpanningWeightByDisjointSet();

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
        Classify(graph, BfsConnectivityWeight);

    // The same three-probe classification over this repo's DisjointSet.
    public static (int[] Critical, int[] PseudoCritical) ClassifyEdgesByDisjointSet(
        int nodeCount, int[][] edges)
    {
        var graph = WeightedEdgeList.Build(nodeCount, edges);

        return ClassifyEdgesByDisjointSet(graph);
    }

    public static (int[] Critical, int[] PseudoCritical) ClassifyEdgesByDisjointSet(WeightedEdgeList graph) =>
        Classify(graph, DisjointSetWeight);

    private static (int[] Critical, int[] PseudoCritical) Classify(
        WeightedEdgeList graph, ISpanningWeight spanningWeight)
    {
        var baseline = spanningWeight.Compute(graph, EdgeProbe.Unconstrained)
            ?? throw new InvalidOperationException("LeetCode 1489 guarantees a connected input graph.");

        var critical = new List<int>();
        var pseudoCritical = new List<int>();

        for (var index = 0; index < graph.Edges.Length; index++)
        {
            if (IsCritical(graph, spanningWeight, index, baseline))
            {
                critical.Add(index);
            }
            else if (spanningWeight.Compute(graph, EdgeProbe.Forcing(index)) == baseline)
            {
                pseudoCritical.Add(index);
            }
        }

        return ([.. critical], [.. pseudoCritical]);
    }

    // Dropping a critical edge either leaves the graph disconnected (no spanning weight
    // at all) or forces a heavier tree.
    private static bool IsCritical(
        WeightedEdgeList graph, ISpanningWeight spanningWeight, int index, int baseline)
    {
        var withoutEdge = spanningWeight.Compute(graph, EdgeProbe.Skipping(index));

        return withoutEdge is null || withoutEdge > baseline;
    }

    // The one question the two arms answer differently: the weight of the minimum
    // spanning tree under one probe's constraint. Both of its inputs are named here,
    // and what a null means - not "weight zero" but "the constrained graph has no
    // spanning tree at all" - has somewhere to be stated.
    private interface ISpanningWeight
    {
        int? Compute(WeightedEdgeList graph, EdgeProbe probe);
    }

    private sealed class SpanningWeightByDisjointSet : ISpanningWeight
    {
        public int? Compute(WeightedEdgeList graph, EdgeProbe probe)
        {
            var forest = (Components: new DisjointSet(graph.NodeCount), Tally: new SpanningTally());

            if (probe.ForceIndex != NoEdge)
            {
                UnionEdge(forest, graph.Edges[probe.ForceIndex]);
            }

            foreach (var index in graph.ByWeight)
            {
                TryUnionEdge(graph, forest, index, probe);
            }

            return forest.Tally.SpanningWeightOf(graph.NodeCount);
        }
    }

    // The scan's own state: the components whose connectivity answers the question,
    // and the running tally of the tree they have accepted - both built here, passed
    // together to every accept step, and never meaningful apart.
    private static void TryUnionEdge(
        WeightedEdgeList graph, (DisjointSet Components, SpanningTally Tally) forest, int index, EdgeProbe probe)
    {
        if (probe.Excludes(index))
        {
            return;
        }

        var edge = graph.Edges[index];

        if (!forest.Components.IsConnected(edge[0], edge[1]))
        {
            UnionEdge(forest, edge);
        }
    }

    private static void UnionEdge((DisjointSet Components, SpanningTally Tally) forest, int[] edge)
    {
        forest.Components.Union(edge[0], edge[1]);
        forest.Tally.Accept(WeightedEdgeList.WeightOf(edge));
    }

    private sealed class SpanningWeightByBfs : ISpanningWeight
    {
        public int? Compute(WeightedEdgeList graph, EdgeProbe probe)
        {
            var adjacency = new List<int>[graph.NodeCount];

            for (var node = 0; node < graph.NodeCount; node++)
            {
                adjacency[node] = [];
            }

            // The same scan state as the DisjointSet arm above, with the adjacency list
            // standing in for the union-find that answers the connectivity question.
            var forest = (Adjacency: adjacency, Tally: new SpanningTally());

            if (probe.ForceIndex != NoEdge)
            {
                AcceptBfsEdge(forest, graph.Edges[probe.ForceIndex]);
            }

            foreach (var index in graph.ByWeight)
            {
                TryAcceptBfsEdge(graph, forest, index, probe);
            }

            return forest.Tally.SpanningWeightOf(graph.NodeCount);
        }
    }

    private static void TryAcceptBfsEdge(
        WeightedEdgeList graph, (List<int>[] Adjacency, SpanningTally Tally) forest, int index, EdgeProbe probe)
    {
        if (probe.Excludes(index))
        {
            return;
        }

        var edge = graph.Edges[index];

        if (!ReachableViaBfs(forest.Adjacency, edge[0], edge[1]))
        {
            AcceptBfsEdge(forest, edge);
        }
    }

    private static void AcceptBfsEdge((List<int>[] Adjacency, SpanningTally Tally) forest, int[] edge)
    {
        forest.Adjacency[edge[0]].Add(edge[1]);
        forest.Adjacency[edge[1]].Add(edge[0]);
        forest.Tally.Accept(WeightedEdgeList.WeightOf(edge));
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
        public int? SpanningWeightOf(int nodeCount) => HasSpanningTree(nodeCount) ? _totalWeight : null;

        // The edges accepted so far form a spanning tree once there are one fewer of
        // them than there are nodes - Kruskal never accepts an edge that closes a cycle.
        private bool HasSpanningTree(int nodeCount) => _edgesUsed == nodeCount - 1;
    }
}
