using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths;

public sealed partial class ShortestPathTests
{
    [Fact]
    public void Dijkstra_ComputesCorrectDistancesFromSource()
    {
        var (a, b, c, d) = WeightedGraphs.SampleGraph();

        var distances = ShortestPath.Dijkstra<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(a);

        Assert.Equal(0, distances[a]);
        Assert.Equal(1, distances[b]);
        Assert.Equal(3, distances[c]); // A-B-C = 1+2, cheaper than direct A-C = 4
        Assert.Equal(4, distances[d]); // A-B-C-D = 1+2+1, cheaper than A-C-D = 5 or A-B-D = 6
    }

    [Fact]
    public void Dijkstra_UnreachableNode_IsAbsentFromResult()
    {
        var (a, _, _, _) = WeightedGraphs.SampleGraph();
        var unreachable = new WeightedNode("Z");

        var distances = ShortestPath.Dijkstra<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(a);

        Assert.False(distances.ContainsKey(unreachable));
    }

    [Fact]
    public void AStar_WithZeroHeuristic_AgreesWithDijkstra()
    {
        // ZeroHeuristic makes every candidate's priority equal its real accumulated
        // distance, same as Dijkstra's - proof AStar isn't a parallel
        // implementation, it's the same Explore core under a different THeuristic.
        var (a, _, _, d) = WeightedGraphs.SampleGraph();

        var distance = ShortestPath.AStar<
            WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int, ZeroHeuristic<WeightedNode, int>>(
            a, d);

        Assert.Equal(4, distance);
    }

    [Fact]
    public void AStar_RoutesAroundAWall_MatchingDijkstrasDistance()
    {
        var nodes = WeightedGrids.WithCenterWall();
        var start = nodes[(1, 0)];
        var target = nodes[(1, 2)];

        var aStarDistance = ShortestPath.AStar<
            WeightedGridNode, WeightedGridTopology, ListEdges<WeightedGridNode, int>, int, ManhattanHeuristic>(
            start, target);

        var dijkstraDistances = ShortestPath.Dijkstra<
            WeightedGridNode, WeightedGridTopology, ListEdges<WeightedGridNode, int>, int>(start);

        // The wall makes the true distance (4) longer than the heuristic's
        // unobstructed straight-line estimate (2) - AStar still has to find the real
        // detour, not just report the heuristic.
        Assert.Equal(4, aStarDistance);
        Assert.Equal(dijkstraDistances[target], aStarDistance);
    }

    [Fact]
    public void AStar_UnreachableTarget_ReturnsNull()
    {
        var nodes = WeightedGrids.WithCenterWall();
        var start = nodes[(0, 0)];
        var unreachable = new WeightedGridNode("unreachable", 99, 99);

        var distance = ShortestPath.AStar<
            WeightedGridNode, WeightedGridTopology, ListEdges<WeightedGridNode, int>, int, ManhattanHeuristic>(
            start, unreachable);

        Assert.Null(distance);
    }

    [Fact]
    public void EdgeTopologyAsGraphTopology_LetsWeightedTopologyBeUsedByPlainGraphReduce()
    {
        // Same graph, weights discarded entirely via the generic projection - no
        // hand-written wrapper, and proves it genuinely composes with the
        // pre-existing, weight-agnostic algorithms rather than needing its own
        // parallel implementation.
        var (a, _, _, _) = WeightedGraphs.SampleGraph();

        var count = Reduce.Graph<
            WeightedNode,
            EdgeTopologyAsGraphTopology<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>,
            EdgeTargets<WeightedNode, int, ListEdges<WeightedNode, int>>,
            NaturalChildOrder<WeightedNode, EdgeTargets<WeightedNode, int, ListEdges<WeightedNode, int>>>,
            EdgeTargets<WeightedNode, int, ListEdges<WeightedNode, int>>,
            BreadthFirstReduceOrder<WeightedNode>,
            CountWeightedNodesReduceAlgebra,
            int>(a);

        Assert.Equal(4, count);
    }
}
