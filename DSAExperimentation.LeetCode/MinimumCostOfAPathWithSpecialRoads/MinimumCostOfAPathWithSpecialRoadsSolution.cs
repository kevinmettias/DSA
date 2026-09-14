using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimumCostOfAPathWithSpecialRoads;

// LeetCode 2662. Minimum Cost of a Path With Special Roads: walking from (x1,y1) to
// (x2,y2) always costs their Manhattan distance, and each special road offers a
// one-way discounted jump between two given points. A route only ever needs to turn
// at the start, the target, or a special road's endpoint, so the whole plane
// collapses to that finite point set - a complete graph carrying a symmetric
// Manhattan edge between every pair, plus each special road's own directed
// discounted edge - and the answer is the shortest distance from start to target.
//
// Both strategies below build exactly that graph and differ only in how they settle
// it. The graph is dense (every pair of the ~2*roads+2 points is directly connected),
// which is the well-known case where the array-scan baseline beats a heap - see
// MinimumCostOfAPathWithSpecialRoadsBenchmarks.
//
// No prepared-input overload (section 17.4): the benchmark's [GlobalSetup] hoists only
// the randomly generated start/target/specialRoads arrays, which are already
// LeetCode's own argument shape, and each strategy's own representation of the point
// graph is part of what that strategy costs - handing both arms one shared pre-built
// graph would measure neither of them.
internal static class MinimumCostOfAPathWithSpecialRoadsSolution
{
    // A point is [x, y]; a special road is [x1, y1, x2, y2, cost], whose first two
    // columns are its from-point in exactly that layout.
    private const int XIndex = 0;
    private const int YIndex = 1;
    private const int ToXIndex = 2;
    private const int ToYIndex = 3;
    private const int CostIndex = 4;

    // The textbook answer: the O(V^2) array-scan Dijkstra, with no priority queue and
    // no materialized adjacency for the dense Manhattan edges - they are recomputed
    // from the coordinate formula on every relaxation round. Deliberately written
    // without this repo's primitives; it is the arm the composed strategy below has to
    // justify itself against.
    public static int MinimumCostByArrayScanDijkstra(int[] start, int[] target, int[][] specialRoads)
    {
        var points = CollectDistinctPoints(start, target, specialRoads);
        var pointIndex = BuildPointIndex(points);
        var specialEdgesByFrom = BuildSpecialEdgesByFromIndex(points.Length, pointIndex, specialRoads);
        var startIndex = pointIndex[(start[XIndex], start[YIndex])];
        var targetIndex = pointIndex[(target[XIndex], target[YIndex])];

        return RunArrayScanDijkstra(points, specialEdgesByFrom, startIndex, targetIndex);
    }

    private static (int X, int Y)[] CollectDistinctPoints(int[] start, int[] target, int[][] specialRoads)
    {
        var seen = new HashSet<(int X, int Y)>();
        var points = new List<(int X, int Y)>();

        AddPoint(seen, points, (start[XIndex], start[YIndex]));
        AddPoint(seen, points, (target[XIndex], target[YIndex]));

        foreach (var road in specialRoads)
        {
            AddPoint(seen, points, (road[XIndex], road[YIndex]));
            AddPoint(seen, points, (road[ToXIndex], road[ToYIndex]));
        }

        return [.. points];
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

    private static (int To, int Cost)[][] BuildSpecialEdgesByFromIndex(
        int pointCount, Dictionary<(int X, int Y), int> pointIndex, int[][] specialRoads)
    {
        var edgesByFrom = new List<(int To, int Cost)>[pointCount];

        for (var i = 0; i < pointCount; i++)
        {
            edgesByFrom[i] = [];
        }

        foreach (var road in specialRoads)
        {
            var from = pointIndex[(road[XIndex], road[YIndex])];
            var to = pointIndex[(road[ToXIndex], road[ToYIndex])];
            edgesByFrom[from].Add((to, road[CostIndex]));
        }

        return [.. edgesByFrom.Select(edges => edges.ToArray())];
    }

    private static int RunArrayScanDijkstra(
        (int X, int Y)[] points, (int To, int Cost)[][] specialEdgesByFrom, int startIndex, int targetIndex)
    {
        var distances = new int[points.Length];
        Array.Fill(distances, int.MaxValue);
        distances[startIndex] = 0;
        var visited = new bool[points.Length];

        for (var iteration = 0; iteration < points.Length; iteration++)
        {
            var closest = SelectClosestUnvisited(distances, visited);

            if (closest < 0)
            {
                break;
            }

            visited[closest] = true;
            RelaxManhattanEdges(points, distances, closest);
            RelaxSpecialEdges(specialEdgesByFrom[closest], distances, closest);
        }

        return distances[targetIndex];
    }

    private static int SelectClosestUnvisited(int[] distances, bool[] visited)
    {
        var closest = -1;

        for (var v = 0; v < distances.Length; v++)
        {
            if (IsNearerUnsettled(distances, visited, v, closest))
            {
                closest = v;
            }
        }

        return closest;
    }

    // The array-scan's own "extract-min": a candidate replaces the incumbent when it
    // is still unsettled, has been reached at all, and is nearer than whatever the
    // scan is currently holding (anything reached beats "nothing found yet").
    private static bool IsNearerUnsettled(int[] distances, bool[] visited, int candidate, int incumbent)
    {
        if (visited[candidate] || distances[candidate] == int.MaxValue)
        {
            return false;
        }

        return incumbent < 0 || distances[candidate] < distances[incumbent];
    }

    private static void RelaxManhattanEdges((int X, int Y)[] points, int[] distances, int from)
    {
        for (var v = 0; v < points.Length; v++)
        {
            if (v == from)
            {
                continue;
            }

            var candidate = distances[from] + ManhattanDistance(points[from], points[v]);

            if (candidate < distances[v])
            {
                distances[v] = candidate;
            }
        }
    }

    private static void RelaxSpecialEdges((int To, int Cost)[] edges, int[] distances, int from)
    {
        foreach (var (to, cost) in edges)
        {
            var candidate = distances[from] + cost;

            if (candidate < distances[to])
            {
                distances[to] = candidate;
            }
        }
    }

    // This repo's own answer: materialize the same point graph onto
    // SpecialRoadNode/SpecialRoadTopology and hand it to ShortestPath.Dijkstra, whose
    // settled distance map already answers "cheapest route to every point" - so the
    // problem is one lookup in the result.
    public static int MinimumCostByHeapDijkstra(int[] start, int[] target, int[][] specialRoads)
    {
        var nodesByPoint = new Dictionary<(int X, int Y), SpecialRoadNode>();
        var startNode = GetOrCreateNode(nodesByPoint, (start[XIndex], start[YIndex]));
        var targetNode = GetOrCreateNode(nodesByPoint, (target[XIndex], target[YIndex]));

        foreach (var road in specialRoads)
        {
            GetOrCreateNode(nodesByPoint, (road[XIndex], road[YIndex]));
            GetOrCreateNode(nodesByPoint, (road[ToXIndex], road[ToYIndex]));
        }

        WireCompleteManhattanGraph(nodesByPoint);
        WireSpecialRoadEdges(nodesByPoint, specialRoads);

        var distances = ShortestPath.Dijkstra<
            SpecialRoadNode, SpecialRoadTopology, ListEdges<SpecialRoadNode, int>, int>(startNode);

        return distances[targetNode];
    }

    private static SpecialRoadNode GetOrCreateNode(
        Dictionary<(int X, int Y), SpecialRoadNode> nodesByPoint, (int X, int Y) point)
    {
        if (!nodesByPoint.TryGetValue(point, out var node))
        {
            node = new SpecialRoadNode(nodesByPoint.Count);
            nodesByPoint[point] = node;
        }

        return node;
    }

    private static void WireCompleteManhattanGraph(Dictionary<(int X, int Y), SpecialRoadNode> nodesByPoint)
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

    private static void WireSpecialRoadEdges(
        Dictionary<(int X, int Y), SpecialRoadNode> nodesByPoint, int[][] specialRoads)
    {
        foreach (var road in specialRoads)
        {
            var from = nodesByPoint[(road[XIndex], road[YIndex])];
            var to = nodesByPoint[(road[ToXIndex], road[ToYIndex])];
            from.Edges.Add((road[CostIndex], to));
        }
    }

    private static int ManhattanDistance((int X, int Y) from, (int X, int Y) to)
        => Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
}
