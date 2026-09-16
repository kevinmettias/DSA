using DSAExperimentation.Algorithms.MinimumSpanningTrees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinCostToConnectAllPoints;

// LeetCode 1584. Min Cost to Connect All Points: the minimum total edge weight
// that leaves every point connected, with the weight of an edge defined as the
// Manhattan distance between its two points. Any pair of points may be joined, so
// the graph is complete - one edge per pair - and "cheapest connecting subgraph"
// is by definition a minimum spanning tree's total weight.
//
// Both strategies below compute that same total. They differ only in whether the
// complete graph is ever materialized:
//
//   - MinCostConnectPointsByDensePrim is the textbook dense-graph Prim's, O(n^2)
//     with no heap and no edge list: it keeps one "cheapest known edge into the
//     tree" per point in a flat array and rescans that array for the minimum.
//     Deliberately written with nothing but BCL arrays - it is the arm the
//     composed solution has to justify itself against.
//   - MinCostConnectPointsByKruskalMst materializes all n(n-1)/2 edges and hands
//     them to this repo's own Algorithms.MinimumSpanningTrees.MinimumSpanningTree
//     .Kruskal, which sorts them and unions endpoints through a KeyedDisjointSet.
//     Asymptotically O(n^2 log n) here precisely because the graph is dense, which
//     is the comparison MinCostToConnectAllPointsBenchmarks exists to measure.
internal static class MinCostToConnectAllPointsSolution
{
    // The textbook answer for this exact "complete graph over points" shape: grow
    // the tree one point at a time, always attaching the unattached point whose
    // cheapest edge into the tree is smallest, recomputing distances on the fly so
    // no edge is ever stored.
    public static int MinCostConnectPointsByDensePrim(int[][] points)
    {
        var count = points.Length;

        if (count == 0)
        {
            return 0;
        }

        var inTree = new bool[count];
        var cheapestEdge = new int[count];
        Array.Fill(cheapestEdge, int.MaxValue);
        cheapestEdge[0] = 0;

        var total = 0;

        for (var attached = 0; attached < count; attached++)
        {
            total += AttachNearestPoint(points, inTree, cheapestEdge);
        }

        return total;
    }

    private static int AttachNearestPoint(int[][] points, bool[] inTree, int[] cheapestEdge)
    {
        var next = FindNearestOutsideTree(inTree, cheapestEdge);

        inTree[next] = true;
        RelaxEdgesFrom(points, inTree, cheapestEdge, next);

        return cheapestEdge[next];
    }

    private static int FindNearestOutsideTree(bool[] inTree, int[] cheapestEdge)
    {
        var next = -1;

        for (var candidate = 0; candidate < inTree.Length; candidate++)
        {
            if (IsCheaperOutsideTree(inTree, cheapestEdge, candidate, next))
            {
                next = candidate;
            }
        }

        return next;
    }

    // The candidate takes over as the next attachment when it is still outside the
    // tree and either nothing has been chosen yet or it reaches the tree more
    // cheaply than the current best does.
    private static bool IsCheaperOutsideTree(bool[] inTree, int[] cheapestEdge, int candidate, int next)
        => !inTree[candidate] && (next == -1 || cheapestEdge[candidate] < cheapestEdge[next]);

    private static void RelaxEdgesFrom(int[][] points, bool[] inTree, int[] cheapestEdge, int attached)
    {
        for (var candidate = 0; candidate < points.Length; candidate++)
        {
            if (inTree[candidate])
            {
                continue;
            }

            var distance = ManhattanDistance(points[attached], points[candidate]);

            if (distance < cheapestEdge[candidate])
            {
                cheapestEdge[candidate] = distance;
            }
        }
    }

    // This repo's own MST: build the complete Manhattan-distance graph over the
    // points and read off the total weight of the spanning forest Kruskal returns.
    // The input is guaranteed connected (every pair has an edge), so the forest is
    // always a single tree of exactly n-1 edges.
    //
    // No prepared-input overload (§17.4): the benchmark's [GlobalSetup] hoists the
    // points themselves, which are already LeetCode's own input shape, and building
    // the complete graph is the very cost this arm is being measured for - charging
    // it to setup would hide the difference the comparison exists to show.
    public static int MinCostConnectPointsByKruskalMst(int[][] points)
    {
        var spanningEdges = MinimumSpanningTree.Kruskal<
            PointNode, PointTopology, ListEdges<PointNode, int>, int>(BuildCompleteGraph(points));

        return spanningEdges.Sum(edge => edge.Weight);
    }

    private static List<PointNode> BuildCompleteGraph(int[][] points)
    {
        var nodes = new List<PointNode>(points.Length);

        for (var index = 0; index < points.Length; index++)
        {
            nodes.Add(new PointNode(index));
        }

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                var weight = ManhattanDistance(points[i], points[j]);
                nodes[i].Edges.Add((weight, nodes[j]));
                nodes[j].Edges.Add((weight, nodes[i]));
            }
        }

        return nodes;
    }

    private static int ManhattanDistance(int[] firstPoint, int[] secondPoint) =>
        Math.Abs(firstPoint[0] - secondPoint[0]) + Math.Abs(firstPoint[1] - secondPoint[1]);
}
