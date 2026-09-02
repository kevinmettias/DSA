using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Cost of a Path With Special Roads (LC 2662): start, target, and every
// special road's two endpoints become nodes in a small weighted graph - a direct
// (Manhattan-distance) edge between every pair of nodes plus each special road's own
// one-way discounted edge - and the answer is the shortest distance from start to
// target. ArrayDijkstra is the textbook O(V^2) array-scan Dijkstra (no priority
// queue, edges computed on the fly from the Manhattan formula) run directly over the
// point array; RepoHeapDijkstra builds that same edge set onto
// WeightedGraphNode/WeightedGraphTopology (this repo's own graph fixtures, already
// used by the Dijkstra/BellmanFord/FloydWarshall benchmarks) and hands it to
// ShortestPath.Dijkstra - the repo-primitive composition
// MinimumCostOfAPathWithSpecialRoadsTests proves correct. The graph here is dense
// (every pair of the ~2*SpecialRoadCount+2 points is directly connected), so the
// array-scan baseline is expected to win - it's the well-known dense-graph case
// where a heap's O(log V) bookkeeping costs more than it saves.
[MemoryDiagnoser]
public class MinimumCostOfAPathWithSpecialRoadsBenchmarks
{
    private const int Seed = 2662; // LC problem number
    private const int CoordinateUpperBound = 1_000;
    private const int CostUpperBound = 500;

    [Params(20, 100)]
    public int SpecialRoadCount;

    private (int X, int Y) _start;
    private (int X, int Y) _target;
    private (int X1, int Y1, int X2, int Y2, int Cost)[] _specialRoads = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _start = (random.Next(CoordinateUpperBound), random.Next(CoordinateUpperBound));
        _target = (random.Next(CoordinateUpperBound), random.Next(CoordinateUpperBound));
        _specialRoads = Enumerable.Range(0, SpecialRoadCount)
            .Select(_ => (
                random.Next(CoordinateUpperBound), random.Next(CoordinateUpperBound),
                random.Next(CoordinateUpperBound), random.Next(CoordinateUpperBound),
                random.Next(1, CostUpperBound)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ArrayDijkstra()
    {
        var points = CollectDistinctPoints();
        var pointIndex = BuildPointIndex(points);
        var specialEdgesByFrom = BuildSpecialEdgesByFromIndex(points, pointIndex);

        return RunArrayDijkstra(points, specialEdgesByFrom, pointIndex[_start], pointIndex[_target]);
    }

    [Benchmark]
    public int RepoHeapDijkstra()
    {
        var nodesByPoint = new Dictionary<(int X, int Y), WeightedGraphNode>();
        var startNode = GetOrCreateNode(nodesByPoint, _start);
        var targetNode = GetOrCreateNode(nodesByPoint, _target);

        foreach (var road in _specialRoads)
        {
            GetOrCreateNode(nodesByPoint, (road.X1, road.Y1));
            GetOrCreateNode(nodesByPoint, (road.X2, road.Y2));
        }

        WireCompleteManhattanGraph(nodesByPoint);
        WireSpecialRoadEdges(nodesByPoint);

        var distances = ShortestPath.Dijkstra<WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(startNode);

        return distances[targetNode];
    }

    private (int X, int Y)[] CollectDistinctPoints()
    {
        var seen = new HashSet<(int X, int Y)>();
        var points = new List<(int X, int Y)>();

        AddPoint(seen, points, _start);
        AddPoint(seen, points, _target);

        foreach (var road in _specialRoads)
        {
            AddPoint(seen, points, (road.X1, road.Y1));
            AddPoint(seen, points, (road.X2, road.Y2));
        }

        return points.ToArray();
    }

    private static void AddPoint(HashSet<(int X, int Y)> seen, List<(int X, int Y)> points, (int X, int Y) point)
    {
        if (seen.Add(point))
        {
            points.Add(point);
        }
    }

    private static Dictionary<(int X, int Y), int> BuildPointIndex((int X, int Y)[] points)
    {
        var index = new Dictionary<(int X, int Y), int>();

        for (var i = 0; i < points.Length; i++)
        {
            index[points[i]] = i;
        }

        return index;
    }

    private (int To, int Cost)[][] BuildSpecialEdgesByFromIndex(
        (int X, int Y)[] points, Dictionary<(int X, int Y), int> pointIndex)
    {
        var edgesByFrom = new List<(int To, int Cost)>[points.Length];
        for (var i = 0; i < points.Length; i++)
        {
            edgesByFrom[i] = [];
        }

        foreach (var road in _specialRoads)
        {
            var from = pointIndex[(road.X1, road.Y1)];
            var to = pointIndex[(road.X2, road.Y2)];
            edgesByFrom[from].Add((to, road.Cost));
        }

        return edgesByFrom.Select(edges => edges.ToArray()).ToArray();
    }

    private static int RunArrayDijkstra(
        (int X, int Y)[] points, (int To, int Cost)[][] specialEdgesByFrom, int startIndex, int targetIndex)
    {
        var dist = new int[points.Length];
        Array.Fill(dist, int.MaxValue);
        dist[startIndex] = 0;
        var visited = new bool[points.Length];

        for (var iteration = 0; iteration < points.Length; iteration++)
        {
            var u = SelectClosestUnvisited(dist, visited);
            if (u < 0)
            {
                break;
            }

            visited[u] = true;
            RelaxManhattanEdges(points, dist, u);
            RelaxSpecialEdges(specialEdgesByFrom[u], dist, u);
        }

        return dist[targetIndex];
    }

    private static int SelectClosestUnvisited(int[] dist, bool[] visited)
    {
        var u = -1;

        for (var v = 0; v < dist.Length; v++)
        {
            if (!visited[v] && dist[v] != int.MaxValue && (u < 0 || dist[v] < dist[u]))
            {
                u = v;
            }
        }

        return u;
    }

    private static void RelaxManhattanEdges((int X, int Y)[] points, int[] dist, int u)
    {
        for (var v = 0; v < points.Length; v++)
        {
            if (v == u)
            {
                continue;
            }

            var candidate = dist[u] + ManhattanDistance(points[u], points[v]);
            if (candidate < dist[v])
            {
                dist[v] = candidate;
            }
        }
    }

    private static void RelaxSpecialEdges((int To, int Cost)[] edges, int[] dist, int u)
    {
        foreach (var (to, cost) in edges)
        {
            var candidate = dist[u] + cost;
            if (candidate < dist[to])
            {
                dist[to] = candidate;
            }
        }
    }

    private static void WireCompleteManhattanGraph(Dictionary<(int X, int Y), WeightedGraphNode> nodesByPoint)
    {
        var points = nodesByPoint.ToArray();

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                var distance = ManhattanDistance(points[i].Key, points[j].Key);
                points[i].Value.Edges.Add((distance, points[j].Value));
                points[j].Value.Edges.Add((distance, points[i].Value));
            }
        }
    }

    private void WireSpecialRoadEdges(Dictionary<(int X, int Y), WeightedGraphNode> nodesByPoint)
    {
        foreach (var road in _specialRoads)
        {
            var from = nodesByPoint[(road.X1, road.Y1)];
            var to = nodesByPoint[(road.X2, road.Y2)];
            from.Edges.Add((road.Cost, to));
        }
    }

    private static WeightedGraphNode GetOrCreateNode(
        Dictionary<(int X, int Y), WeightedGraphNode> nodesByPoint, (int X, int Y) point)
    {
        if (!nodesByPoint.TryGetValue(point, out var node))
        {
            node = new WeightedGraphNode(nodesByPoint.Count);
            nodesByPoint[point] = node;
        }

        return node;
    }

    private static int ManhattanDistance((int X, int Y) a, (int X, int Y) b)
        => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
}
