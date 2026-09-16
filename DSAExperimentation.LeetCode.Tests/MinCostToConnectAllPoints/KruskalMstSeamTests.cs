using DSAExperimentation.Algorithms.MinimumSpanningTrees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.LeetCode.MinCostToConnectAllPoints;

namespace DSAExperimentation.LeetCode.Tests.MinCostToConnectAllPoints;

// The seam between MinCostToConnectAllPointsSolution's two strategies.
// MinCostConnectPointsByDensePrim keeps one cheapest-known-edge per point in a flat
// array and never stores a graph. MinCostConnectPointsByKruskalMst builds the whole
// complete Manhattan-distance graph out of PointNode/PointTopology - the problem's own
// IEdgeTopology witness - and hands it to this repo's own
// Algorithms.MinimumSpanningTrees.MinimumSpanningTree.Kruskal, which sorts the
// candidate edges and unions endpoints through DataStructures.KeyedDisjointSet.
//
// The seam has two halves that the solution's own answer only reports one of. Every
// edge of that graph is listed twice, once in each endpoint's adjacency list, so
// Kruskal's candidate list is twice the edge count and the forest it returns must
// still be one tree of n-1 edges - the DSA structure's own "changes cost, not output"
// claim. And the vertex list Kruskal is handed is the whole point set, so a node whose
// neighbor falls outside it is a case KeyedDisjointSet resolves by exclusion rather
// than by throwing. Both halves are asserted here against Kruskal directly, because
// the solution only ever reads the sum of the edge weights.
public sealed partial class KruskalMstSeamTests
{
    [Fact]
    public void MinCost_LeetCodeExample_MatchesDensePrim()
    {
        int[][] points = [[0, 0], [2, 2], [3, 10], [5, 2], [7, 0]];

        Assert.Equal(20, MinCostByKruskalArm(points));
        AssertSameCost(points);
    }

    // The forest Kruskal returns for a connected complete graph has to be a single
    // spanning tree: n-1 edges, no more. The solution's answer is the sum alone, so an
    // extra rejected-but-kept edge or a missing one would only show up as a wrong
    // total on a graph where the arithmetic happened to hide it.
    [Fact]
    public void Kruskal_CompleteGraphOverPoints_ReturnsSpanningTreeWithOneFewerEdgeThanVertices()
    {
        int[][] points = [[0, 0], [4, 0], [4, 3], [0, 3], [2, 2]];
        var nodes = BuildCompleteGraph(points);

        var forest = MinimumSpanningTree.Kruskal<PointNode, PointTopology, ListEdges<PointNode, int>, int>(nodes);

        Assert.Equal(points.Length - 1, forest.Count);
        Assert.Equal(MinCostByKruskalArm(points), forest.Sum(edge => edge.Weight));
    }

    // The same complete graph with each edge listed only once, from the lower
    // endpoint's side. Kruskal must return the same forest size and the same total:
    // the duplicate discovery is rejected by the disjoint set, so it can change how
    // many candidates are sorted but never which edges end up in the tree.
    [Fact]
    public void Kruskal_EachEdgeListedOnce_ReturnsTheSameForestAsSymmetricAdjacency()
    {
        int[][] points = [[0, 0], [1, 5], [5, 1], [6, 6], [3, 3]];
        var symmetric = BuildCompleteGraph(points);
        var oneSided = BuildOneSidedGraph(points);

        var symmetricForest = MinimumSpanningTree.Kruskal<PointNode, PointTopology, ListEdges<PointNode, int>, int>(symmetric);
        var oneSidedForest = MinimumSpanningTree.Kruskal<PointNode, PointTopology, ListEdges<PointNode, int>, int>(oneSided);

        Assert.Equal(symmetricForest.Count, oneSidedForest.Count);
        Assert.Equal(
            symmetricForest.Sum(edge => edge.Weight),
            oneSidedForest.Sum(edge => edge.Weight));
        AssertSameCost(points);
    }

    // A vertex list that omits a point the listed nodes have edges to: the omitted
    // point is not a key in Kruskal's KeyedDisjointSet, so every edge reaching it is
    // excluded rather than thrown on, and what comes back is the forest over the
    // points that were actually listed.
    [Fact]
    public void Kruskal_VertexListMissingAnEndpoint_ExcludesThoseEdgesRatherThanThrowing()
    {
        int[][] points = [[0, 0], [3, 4], [100, 100]];
        var nodes = BuildCompleteGraph(points);

        var forest = MinimumSpanningTree.Kruskal<PointNode, PointTopology, ListEdges<PointNode, int>, int>(nodes.GetRange(0, 2));

        Assert.Single(forest);
        Assert.Equal(7, forest.Sum(edge => edge.Weight));
    }

    // A vertex list that repeats a point: KeyedDisjointSet assigns each distinct key
    // one id, so the repeated point is still one component and the forest keeps its
    // n-1 edge count instead of growing a second copy of the same vertex.
    [Fact]
    public void Kruskal_VertexListRepeatingAPoint_CountsItOnce()
    {
        int[][] points = [[0, 0], [0, 7], [7, 0]];
        var nodes = BuildCompleteGraph(points);

        var forest = MinimumSpanningTree.Kruskal<PointNode, PointTopology, ListEdges<PointNode, int>, int>([.. nodes, nodes[0]]);

        Assert.Equal(points.Length - 1, forest.Count);
        Assert.Equal(MinCostByKruskalArm(points), forest.Sum(edge => edge.Weight));
    }

    // Every distance in this point set is equal, so the sorted candidate list is all
    // ties and which edges the tree keeps is decided by the sort's tie order alone -
    // the total must not depend on it. Reversing the input reverses that tie order.
    [Fact]
    public void MinCost_AllDistancesEqual_MatchesDensePrimAndIsUnchangedByInputOrder()
    {
        int[][] points = [[0, 0], [2, 0], [4, 0], [6, 0], [8, 0], [10, 0]];
        int[][] reversed = [.. points.Reverse()];

        Assert.Equal(10, MinCostByKruskalArm(points));
        AssertSameCost(points);
        Assert.Equal(MinCostByKruskalArm(points), MinCostByKruskalArm(reversed));
    }

    // Points sharing a coordinate: many distances are equal and the graph is full of
    // zero-length-in-one-axis edges, so the tie handling has to hold up on a shape
    // where the nearest neighbor is ambiguous at almost every step.
    [Fact]
    public void MinCost_PointsSharingCoordinates_MatchesDensePrim()
    {
        int[][] points = [[0, 0], [0, 1], [0, 2], [1, 0], [1, 1], [1, 2], [2, 0], [2, 1], [2, 2]];

        Assert.Equal(8, MinCostByKruskalArm(points));
        AssertSameCost(points);
    }

    // Coordinates at the extreme of the problem's range: the Manhattan distances are
    // large enough that a narrower accumulator or an absolute-value shortcut would
    // show up as a wrong total, and the candidate count is at its largest.
    [Fact]
    public void MinCost_ExtremeCoordinates_MatchesDensePrim()
        => AssertSameCost([[-1000000, -1000000], [1000000, 1000000], [-1000000, 1000000], [1000000, -1000000], [0, 0]]);

    [Fact]
    public void MinCost_SinglePoint_IsZeroFromBothArms()
    {
        int[][] points = [[-1000000, 1000000]];

        Assert.Equal(0, MinCostByKruskalArm(points));
        AssertSameCost(points);
    }

    [Fact]
    public void MinCost_NoPoints_IsZeroFromBothArms()
    {
        int[][] points = [];

        Assert.Equal(0, MinCostByKruskalArm(points));
        AssertSameCost(points);
    }

    private static void AssertSameCost(int[][] points)
        => Assert.Equal(MinCostToConnectAllPointsSolution.MinCostConnectPointsByDensePrim(points), MinCostByKruskalArm(points));

    private static int MinCostByKruskalArm(int[][] points)
        => MinCostToConnectAllPointsSolution.MinCostConnectPointsByKruskalMst(points);

    // The complete Manhattan-distance graph, built the same way the solution builds it:
    // every pair once in each direction, so every edge is discovered twice.
    private static List<PointNode> BuildCompleteGraph(int[][] points)
        => BuildGraph(points, oneSided: false);

    private static List<PointNode> BuildOneSidedGraph(int[][] points)
        => BuildGraph(points, oneSided: true);

    private static List<PointNode> BuildGraph(int[][] points, bool oneSided)
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
                var distance = ManhattanDistance(points[i], points[j]);
                nodes[i].Edges.Add((distance, nodes[j]));

                if (!oneSided)
                {
                    nodes[j].Edges.Add((distance, nodes[i]));
                }
            }
        }

        return nodes;
    }

    private static int ManhattanDistance(int[] firstPoint, int[] secondPoint)
        => Math.Abs(firstPoint[0] - secondPoint[0]) + Math.Abs(firstPoint[1] - secondPoint[1]);
}
