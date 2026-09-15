using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.ModifyGraphEdgeWeights;

// LeetCode 2699. Modify Graph Edge Weights: give every edge marked -1 a positive
// weight so that the shortest distance from source to destination becomes exactly
// target, or report that no assignment can. Any valid assignment is accepted.
//
// Both strategies share the same frame - start every -1 edge at the floor weight
// 1, reject at once if even that floor already overshoots target, accept at once
// if it lands on it, then stretch the -1 edges one at a time, re-running
// ShortestPath.Dijkstra after each change - and differ only in how the weight for
// the edge in hand is picked. That difference is the whole comparison: a linear
// scan pays one Dijkstra pass per candidate weight, while reading the edge's two
// already-settled half-distances gives the exact weight in O(1) extra passes no
// matter how far the stretch has to go.
//
// Neither arm is the usual BCL rewrite (ARCHITECTURE.md 17.5): what the baseline
// exists to show here is the cost of guessing weights against a shortest-path
// oracle, which only means anything if both arms ask the same oracle. LC 1334's
// FindCityByDijkstraPerSource makes the same call for the same reason.
internal static class ModifyGraphEdgeWeightsSolution
{
    // Guess the weight of each -1 edge as 1, 2, 3, ..., re-running the search
    // after every guess until the distance lands on target - O(target) Dijkstra
    // passes per edge, which is what the formula arm below has to beat.
    public static int[][] ModifyEdgeWeightsByLinearWeightScan(
        int n, int[][] edges, (int Source, int Destination, int Target) query)
    {
        var graph = AssignableEdgeGraph.Build(n, edges);
        var search = new WeightSearch(graph, query.Source, query.Destination, query.Target);

        if (TrySettleAtFloor(search, out var settled))
        {
            return settled;
        }

        for (var index = 0; index < graph.EdgeCount; index++)
        {
            if (graph.IsAssignable(index) && ScanWeightUpwards(search, index))
            {
                return graph.Weights();
            }
        }

        return [];
    }

    // Stretching an edge can only push the distance up, so the scan stops the
    // moment it passes target and leaves the edge at the largest weight that kept
    // the distance under it, handing the rest of the stretch to the next -1 edge.
    // No useful weight exceeds target either - a path totalling exactly target
    // cannot spend more than target on one of its own positive-weight edges -
    // which is what bounds the loop.
    private static bool ScanWeightUpwards(WeightSearch search, int edgeIndex)
    {
        var stretched = AssignableEdgeWeights.FloorWeight;

        for (var weight = AssignableEdgeWeights.FloorWeight; weight <= search.Target; weight++)
        {
            search.Graph.SetWeight(edgeIndex, weight);

            if (!TryDistanceToDestination(search, out var distance) || distance > search.Target)
            {
                break;
            }

            stretched = weight;

            if (distance == search.Target)
            {
                return true;
            }
        }

        search.Graph.SetWeight(edgeIndex, stretched);
        return false;
    }

    // The exact weight, read off two searches instead of searched for: whatever
    // target has left over once the settled distance from source to one end and
    // from destination to the other end are both paid for.
    public static int[][] ModifyEdgeWeightsByHalfDistanceFormula(
        int n, int[][] edges, (int Source, int Destination, int Target) query)
    {
        var graph = AssignableEdgeGraph.Build(n, edges);
        var search = new WeightSearch(graph, query.Source, query.Destination, query.Target);

        if (TrySettleAtFloor(search, out var settled))
        {
            return settled;
        }

        return StretchAssignableEdges(graph, search);
    }

    // The frame both arms share: stretch the -1 edges one at a time until one lands the
    // distance on target, or report no assignment when the last one is spent.
    private static int[][] StretchAssignableEdges(AssignableEdgeGraph graph, WeightSearch search)
    {
        for (var index = 0; index < graph.EdgeCount; index++)
        {
            if (!graph.IsAssignable(index))
            {
                continue;
            }

            var (found, answer) = StretchOneEdge(graph, search, index);

            if (found)
            {
                return answer;
            }
        }

        return [];
    }

    // One -1 edge's stretch: price it by the formula, write the weight, and report the
    // graph as the answer when that lands the distance exactly on target. An edge that
    // does not land there reports nothing and the scan moves on to the next one.
    private static (bool Found, int[][] Answer) StretchOneEdge(
        AssignableEdgeGraph graph, WeightSearch search, int index)
    {
        var weight = HalfDistanceWeight(search, index);
        graph.SetWeight(index, weight);

        if (!TryDistanceToDestination(search, out var distance) || distance != search.Target)
        {
            return (false, []);
        }

        return (true, graph.Weights());
    }

    // Two steps rather than one body: read the two settled half-distances the edge in
    // hand is priced from, then turn them into a weight. The read is two Dijkstra
    // passes and the arithmetic is what makes their sum mean something, so each keeps
    // its own name and a reader meets them one at a time.
    private static int HalfDistanceWeight(WeightSearch search, int edgeIndex)
    {
        if (!TrySettledHalfDistances(search, edgeIndex, out var head, out var tail))
        {
            return AssignableEdgeWeights.FloorWeight;
        }

        return WeightLeftForEdge(search, head, tail);
    }

    // Both half-distances come from the graph as it stands, so an edge an earlier
    // iteration already stretched is priced in rather than re-derived. An end that no
    // path has reached yet fails the whole read instead of standing in as zero:
    // pricing an unreachable end as 0 would let the formula invent a weight the graph
    // cannot actually support. Both lookups run even when the first finds nothing -
    // they are pure reads, and a half-distance only means anything as one of a pair.
    private static bool TrySettledHalfDistances(WeightSearch search, int edgeIndex, out int head, out int tail)
    {
        var (from, to) = search.Graph.Endpoints(edgeIndex);
        var fromSource = Distances(search.Graph, search.Source);
        var fromDestination = Distances(search.Graph, search.Destination);
        var headNode = search.Graph.Node(from);
        var tailNode = search.Graph.Node(to);
        var headSettled = fromSource.TryGetValue(headNode, out head);
        var tailSettled = fromDestination.TryGetValue(tailNode, out tail);

        return headSettled && tailSettled;
    }

    // Whatever target has left over once both halves are paid for, and never below the
    // floor every -1 edge is bound to: a distance already at or above target cannot be
    // brought down by a weight no edge is allowed to take.
    private static int WeightLeftForEdge(WeightSearch search, int head, int tail)
    {
        var candidate = search.Target - head - tail;
        return candidate >= AssignableEdgeWeights.FloorWeight ? candidate : AssignableEdgeWeights.FloorWeight;
    }

    // With every -1 edge still at its floor the distance is as short as any legal
    // assignment can make it, so a floor above target is unsatisfiable and a floor
    // exactly on target is already the answer - the one case in which a -1 edge
    // keeps the weight 1 it was built with.
    private static bool TrySettleAtFloor(WeightSearch search, out int[][] settled)
    {
        if (!TryDistanceToDestination(search, out var floor) || floor > search.Target)
        {
            settled = [];
            return true;
        }

        if (floor != search.Target)
        {
            settled = [];
            return false;
        }

        settled = search.Graph.Weights();
        return true;
    }

    private static bool TryDistanceToDestination(WeightSearch search, out int distance)
    {
        var distances = Distances(search.Graph, search.Source);
        var destination = search.Graph.Node(search.Destination);

        return distances.TryGetValue(destination, out distance);
    }

    private static Dictionary<AssignableEdgeNode, int> Distances(AssignableEdgeGraph graph, int source)
    {
        var start = graph.Node(source);

        return ShortestPath
            .Dijkstra<AssignableEdgeNode, AssignableEdgeTopology, ListEdges<AssignableEdgeNode, int>, int>(start);
    }

    // The one graph and the three numbers every step of either strategy needs,
    // carried together so no helper has to restate LeetCode's parameter list.
    private readonly record struct WeightSearch(
        AssignableEdgeGraph Graph,
        int Source,
        int Destination,
        int Target);
}
