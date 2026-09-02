using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReachableNodesInSubdividedGraph;

// LeetCode 882. Reachable Nodes In Subdivided Graph: never materializes the
// subdivided graph itself. Instead it runs this repo's own ShortestPath.Dijkstra
// (NetworkDelayTimeTests' shape, reusing its shared WeightedNode/WeightedTopology
// fixture) over the ORIGINAL graph with each edge weighted at cnt+1 - the number of
// unit moves that edge's subdivision chain actually costs - to get every original
// node's true distance from 0. An original node is reachable directly when its
// distance fits within maxMoves; a subdivision node is reachable when either
// endpoint has enough leftover budget to walk out to it, so each edge's reachable
// subdivision count is max(0, min(cnt, maxMoves - dist[u])) from each side, capped
// at cnt so the two sides' overlap is never double-counted.
public sealed partial class ReachableNodesInSubdividedGraphTests
{
    [Fact]
    public void CountReachableNodes_BudgetSpansMultipleEdges_CountsOriginalAndSubdivisionNodes()
    {
        // 0 -[10 subdivisions]- 1 -[1 subdivision]- 2, plus a direct 0-2 shortcut
        // with 2 subdivisions. dist(0)=0, dist(1)=5 (via 0-2-1, cheaper than the
        // direct 11-hop edge), dist(2)=3.
        int[][] edges = [[0, 1, 10], [1, 2, 1], [0, 2, 2]];

        var count = CountReachableNodes(edges, maxMoves: 6, n: 3);

        Assert.Equal(13, count);
    }

    [Fact]
    public void CountReachableNodes_NoMovesLeft_OnlySourceIsReachable()
    {
        int[][] edges = [[0, 1, 10], [1, 2, 1], [0, 2, 2]];

        var count = CountReachableNodes(edges, maxMoves: 0, n: 3);

        Assert.Equal(1, count);
    }

    [Fact]
    public void CountReachableNodes_NoSubdivisions_BehavesLikePlainReachability()
    {
        int[][] edges = [[0, 1, 0], [1, 2, 0], [0, 2, 0]];

        var count = CountReachableNodes(edges, maxMoves: 1, n: 3);

        Assert.Equal(3, count);
    }

    private static int CountReachableNodes(int[][] edges, int maxMoves, int n)
    {
        var nodes = CreateNodes(n);
        ConnectEdges(nodes, edges);

        var distances = ShortestPath.Dijkstra<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            nodes[0]);

        var reachable = CountDirectlyReachableNodes(nodes, distances, maxMoves);
        reachable += CountSubdivisionsReachable(edges, maxMoves, nodes, distances);
        return reachable;
    }

    private static Dictionary<int, WeightedNode> CreateNodes(int n)
    {
        var nodes = new Dictionary<int, WeightedNode>();

        for (var i = 0; i < n; i++)
        {
            nodes[i] = new WeightedNode(i.ToString());
        }

        return nodes;
    }

    private static void ConnectEdges(Dictionary<int, WeightedNode> nodes, int[][] edges)
    {
        foreach (var edge in edges)
        {
            AddBidirectionalEdge(nodes, edge);
        }
    }

    private static int CountDirectlyReachableNodes(
        Dictionary<int, WeightedNode> nodes, Dictionary<WeightedNode, int> distances, int maxMoves)
    {
        var reachable = 0;

        foreach (var node in nodes.Values)
        {
            if (distances.TryGetValue(node, out var distance) && distance <= maxMoves)
            {
                reachable++;
            }
        }

        return reachable;
    }

    private static int CountSubdivisionsReachable(
        int[][] edges, int maxMoves, Dictionary<int, WeightedNode> nodes, Dictionary<WeightedNode, int> distances)
    {
        var reachable = 0;

        foreach (var edge in edges)
        {
            reachable += CountEdgeSubdivisionsReachable(edge, maxMoves, nodes, distances);
        }

        return reachable;
    }

    private static int CountEdgeSubdivisionsReachable(
        int[] edge, int maxMoves, Dictionary<int, WeightedNode> nodes, Dictionary<WeightedNode, int> distances)
    {
        var (u, v, cnt) = (edge[0], edge[1], edge[2]);
        var fromU = CountReachableSubdivisions(nodes[u], cnt, maxMoves, distances);
        var fromV = CountReachableSubdivisions(nodes[v], cnt, maxMoves, distances);
        return Math.Min(cnt, fromU + fromV);
    }

    private static void AddBidirectionalEdge(Dictionary<int, WeightedNode> nodes, int[] edge)
    {
        var (u, v, cnt) = (edge[0], edge[1], edge[2]);
        var weight = cnt + 1;
        nodes[u].Edges.Add((weight, nodes[v]));
        nodes[v].Edges.Add((weight, nodes[u]));
    }

    // How many of this edge's `cnt` subdivision nodes are reachable by walking out
    // from `node`, given the leftover move budget after reaching it.
    private static int CountReachableSubdivisions(WeightedNode node, int cnt, int maxMoves, Dictionary<WeightedNode, int> distances)
    {
        if (!distances.TryGetValue(node, out var distance))
        {
            return 0;
        }

        var remainingBudget = Math.Min(cnt, maxMoves - distance);
        return Math.Max(0, remainingBudget);
    }
}
