using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToReachDestinationInTime.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToReachDestinationInTime;

// LeetCode 1928. Minimum Cost to Reach Destination in Time: state-expanding over
// (city, elapsedTime) pairs turns this into an ordinary non-negative-edge-weight
// shortest-path problem, so this repo's own ShortestPath.Dijkstra finds the minimum
// fee sum, the same "Dijkstra over an implicit/expanded state graph" shape
// NumberOfRestrictedPathsFromFirstToLastNodeTests already uses for its own
// materialize-then-Dijkstra composition. Every state-graph edge (u,t) -> (v,t+w)
// carries weight passingFees[v]; passingFees[0] is never charged by an incoming
// edge (Dijkstra starts every node's distance at TWeight.Zero), so it is added back
// in once, separately, over the best reachable (n-1, t) distance. Travel times are
// always >= 1 (LeetCode's own constraint), so elapsed time strictly increases along
// every state-graph edge - the state graph is acyclic by construction, which is
// also why a plain BFS expansion bounded by maxTime always terminates instead of
// needing a visited-guard beyond "already materialized this (city, time) pair."
public sealed partial class MinimumCostToReachDestinationInTimeTests
{
    [Fact]
    public void MinCost_ClassicExample_ReturnsCheapestFeeSumWithinBudget()
    {
        int[][] edges = [[0, 1, 10], [1, 2, 10], [2, 5, 10], [0, 3, 1], [3, 4, 10], [4, 5, 15]];
        int[] passingFees = [5, 1, 2, 20, 20, 3];

        var cost = MinCost(maxTime: 30, edges, passingFees);

        Assert.Equal(11, cost);
    }

    [Fact]
    public void MinCost_TightBudgetForcesMoreExpensiveRoute_ReturnsHigherFeeSum()
    {
        int[][] edges = [[0, 1, 10], [1, 2, 10], [2, 5, 10], [0, 3, 1], [3, 4, 10], [4, 5, 15]];
        int[] passingFees = [5, 1, 2, 20, 20, 3];

        var cost = MinCost(maxTime: 29, edges, passingFees);

        Assert.Equal(48, cost);
    }

    [Fact]
    public void MinCost_NoRouteFitsWithinBudget_ReturnsMinusOne()
    {
        int[][] edges = [[0, 1, 10], [1, 2, 10], [2, 5, 10], [0, 3, 1], [3, 4, 10], [4, 5, 15]];
        int[] passingFees = [5, 1, 2, 20, 20, 3];

        var cost = MinCost(maxTime: 25, edges, passingFees);

        Assert.Equal(-1, cost);
    }

    private static int MinCost(int maxTime, int[][] edges, int[] passingFees)
    {
        var n = passingFees.Length;
        var adjacency = BuildAdjacency(n, edges);
        var nodes = new Dictionary<(int City, int Time), TimeCityNode>();
        var start = GetOrCreate(nodes, city: 0, time: 0);

        ExpandReachableStates(new ReachabilityContext(maxTime, passingFees, adjacency, nodes), start);

        var distances = ShortestPath.Dijkstra<
            TimeCityNode, TimeCityEdgeTopology, ListEdges<TimeCityNode, int>, int>(start);

        var best = BestCostToCity(nodes, distances, targetCity: n - 1);

        return best == int.MaxValue ? -1 : passingFees[0] + best;
    }

    private readonly record struct ReachabilityContext(
        int MaxTime, int[] PassingFees, Dictionary<int, List<(int To, int Time)>> Adjacency,
        Dictionary<(int City, int Time), TimeCityNode> Nodes);

    private static void ExpandReachableStates(ReachabilityContext context, TimeCityNode start)
    {
        var frontier = new Queue<TimeCityNode>();
        frontier.Enqueue(start);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            foreach (var edge in context.Adjacency[current.City])
            {
                TryExpandEdge(context, frontier, current, edge);
            }
        }
    }

    private static void TryExpandEdge(ReachabilityContext context, Queue<TimeCityNode> frontier, TimeCityNode current, (int To, int Time) edge)
    {
        var nextTime = current.Time + edge.Time;

        if (nextTime > context.MaxTime)
        {
            return;
        }

        var isNew = !context.Nodes.ContainsKey((edge.To, nextTime));
        var next = GetOrCreate(context.Nodes, edge.To, nextTime);
        current.Edges.Add((context.PassingFees[edge.To], next));

        if (isNew)
        {
            frontier.Enqueue(next);
        }
    }

    private static int BestCostToCity(
        Dictionary<(int City, int Time), TimeCityNode> nodes, Dictionary<TimeCityNode, int> distances, int targetCity)
    {
        var best = int.MaxValue;

        foreach (var (key, node) in nodes)
        {
            if (key.City == targetCity && distances.TryGetValue(node, out var cost) && cost < best)
            {
                best = cost;
            }
        }

        return best;
    }

    private static Dictionary<int, List<(int To, int Time)>> BuildAdjacency(int n, int[][] edges)
    {
        var adjacency = new Dictionary<int, List<(int To, int Time)>>();

        for (var city = 0; city < n; city++)
        {
            adjacency[city] = [];
        }

        foreach (var edge in edges)
        {
            var (from, to, time) = (edge[0], edge[1], edge[2]);
            adjacency[from].Add((to, time));
            adjacency[to].Add((from, time));
        }

        return adjacency;
    }

    private static TimeCityNode GetOrCreate(Dictionary<(int City, int Time), TimeCityNode> nodes, int city, int time)
    {
        var key = (city, time);

        if (!nodes.TryGetValue(key, out var node))
        {
            node = new TimeCityNode(city, time);
            nodes[key] = node;
        }

        return node;
    }
}
