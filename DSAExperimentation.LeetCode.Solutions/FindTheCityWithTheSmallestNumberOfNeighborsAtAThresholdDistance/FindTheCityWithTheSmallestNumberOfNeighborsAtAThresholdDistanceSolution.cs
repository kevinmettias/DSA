using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using CityNode = DSAExperimentation.DataStructures.Graph.Adjacency.WeightedAdjacencyNode<int>;
using CityTopology = DSAExperimentation.DataStructures.Graph.Adjacency.WeightedAdjacencyTopology<int>;

namespace DSAExperimentation.LeetCode.Solutions.FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistance;

// LeetCode 1334. Find the City With the Smallest Number of Neighbors at a
// Threshold Distance: given cityCount cities joined by weighted, undirected roads,
// report the city that can reach the fewest other cities within
// distanceThreshold, breaking ties in favour of the largest city id.
//
// Every strategy answers the identical question - "for every city, how many
// other cities lie within the threshold". The two composed ones differ only in
// which shortest-path primitive they ask, and that is the point of the pair: the
// single-source primitive most shortest-path problems in this repo reach for,
// run once per city, against the one all-pairs primitive that answers the
// question in a single call. The textbook answer, Floyd-Warshall over a plain
// matrix, is what both are measured against. Ties favour the larger id by
// scanning ascending and replacing the best candidate on a <=, in every arm.
internal static class FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceSolution
{
    // Half of int.MaxValue, so two of them still add without overflowing: the distance
    // the matrix holds between cities no road joins.
    private const int NoRoad = int.MaxValue / 2;

    // The textbook answer: Floyd-Warshall over a plain distance matrix, the arm both
    // compositions below are measured against (ARCHITECTURE 17.5).
    public static int FindCityByDistanceMatrix(int cityCount, int[][] edges, int distanceThreshold)
    {
        var distances = RoadDistances(cityCount, edges);

        for (var via = 0; via < cityCount; via++)
        {
            RelaxThrough(distances, via);
        }

        return PickCityWithFewestNeighbors(cityCount, new ArrayMatrixReachableCityCount(distances, distanceThreshold));
    }

    // Each road's own length both ways, nothing between distinct cities no road joins, and
    // nothing from a city to itself.
    private static int[,] RoadDistances(int cityCount, int[][] edges)
    {
        var distances = new int[cityCount, cityCount];

        for (var from = 0; from < cityCount; from++)
        {
            FillRowWithNoRoad(distances, from);
        }

        foreach (var edge in edges)
        {
            var (from, to, length) = (edge[0], edge[1], edge[2]);
            distances[from, to] = Math.Min(distances[from, to], length);
            distances[to, from] = distances[from, to];
        }

        return distances;
    }

    private static void FillRowWithNoRoad(int[,] distances, int from)
    {
        for (var to = 0; to < distances.GetLength(1); to++)
        {
            distances[from, to] = NoRoad;
        }

        distances[from, from] = 0;
    }

    // Lets every pair route through `via` where that is shorter.
    private static void RelaxThrough(int[,] distances, int via)
    {
        var cityCount = distances.GetLength(0);

        for (var from = 0; from < cityCount; from++)
        {
            for (var to = 0; to < cityCount; to++)
            {
                distances[from, to] = Math.Min(distances[from, to], distances[from, via] + distances[via, to]);
            }
        }
    }

    private sealed class ArrayMatrixReachableCityCount(int[,] distances, int distanceThreshold) : IReachableCityCount
    {
        public int CountFrom(int city)
        {
            var count = 0;

            for (var other = 0; other < distances.GetLength(0); other++)
            {
                if (other != city && distances[city, other] <= distanceThreshold)
                {
                    count++;
                }
            }

            return count;
        }
    }

    // The obvious composition, and the arm the all-pairs strategy below has to
    // justify itself against: ShortestPath.Dijkstra from every city in turn,
    // counting that source's own reachable set straight off its distance map.
    // What it exists to show is that reaching for the repo's *single-source*
    // primitive on an all-pairs question is the wrong tool, which only means
    // anything if it really is that primitive.
    public static int FindCityByDijkstraPerSource(int cityCount, int[][] edges, int distanceThreshold)
    {
        var graph = CityGraph.Build(cityCount, edges);

        return FindCityByDijkstraPerSource(graph, distanceThreshold);
    }

    public static int FindCityByDijkstraPerSource(CityGraph graph, int distanceThreshold) =>
        PickCityWithFewestNeighbors(
            graph.Cities.Length,
            new DijkstraReachableCityCount(graph, distanceThreshold));

    private static int CountReachableByDijkstra(CityNode source, int distanceThreshold)
    {
        var distances = ShortestPath
            .Dijkstra<CityNode, CityTopology, ListEdges<CityNode, int>, int>(source);

        return distances.Count(pair => pair.Key != source && pair.Value <= distanceThreshold);
    }

    // The one question the two strategies answer differently: how many other cities a
    // given city reaches within the threshold. The input and the answer are both
    // named here, and the contract a bare `Func<int, int>` had nowhere to state - the
    // city's own entry is never counted, and the count is over distinct cities, not
    // over roads - has somewhere to be written down.
    private interface IReachableCityCount
    {
        int CountFrom(int city);
    }

    private sealed class DijkstraReachableCityCount(CityGraph graph, int distanceThreshold) : IReachableCityCount
    {
        public int CountFrom(int city) => CountReachableByDijkstra(graph.Cities[city], distanceThreshold);
    }

    // AllPairsShortestPaths.TryComputeDistances (Floyd-Warshall) is already
    // exactly "every pair's distance", which is what this problem asks for
    // cityCount times over, so one call replaces the cityCount searches above and
    // every city's count is then a lookup. Unreachable pairs are absent from the map rather
    // than carrying a sentinel, so a missing entry simply fails the threshold
    // test. The bool result reports a negative cycle, which non-negative road
    // lengths make impossible here.
    public static int FindCityByFloydWarshall(int cityCount, int[][] edges, int distanceThreshold)
    {
        var graph = CityGraph.Build(cityCount, edges);

        return FindCityByFloydWarshall(graph, distanceThreshold);
    }

    public static int FindCityByFloydWarshall(CityGraph graph, int distanceThreshold)
    {
        _ = AllPairsShortestPaths.TryComputeDistances<CityNode, CityTopology, ListEdges<CityNode, int>, int>(
            graph.Cities, out var distances);

        return PickCityWithFewestNeighbors(
            graph.Cities.Length,
            new MatrixReachableCityCount(graph, distances, distanceThreshold));
    }

    private static int CountReachableInMatrix(
        CityGraph graph,
        int city,
        Dictionary<(CityNode From, CityNode To), int> distances,
        int distanceThreshold)
    {
        var count = 0;

        for (var other = 0; other < graph.Cities.Length; other++)
        {
            if (other != city
                && IsReachableWithinThreshold(
                    distances, (graph.Cities[city], graph.Cities[other]), distanceThreshold))
            {
                count++;
            }
        }

        return count;
    }

    // A pair can only be counted when the matrix recorded a distance for it at all -
    // unreachable pairs are simply absent - and that distance is within the threshold.
    private static bool IsReachableWithinThreshold(
        Dictionary<(CityNode From, CityNode To), int> distances,
        (CityNode From, CityNode To) pair,
        int distanceThreshold) =>
        distances.TryGetValue(pair, out var distance) && distance <= distanceThreshold;

    private sealed class MatrixReachableCityCount(
        CityGraph graph,
        Dictionary<(CityNode From, CityNode To), int> distances,
        int distanceThreshold) : IReachableCityCount
    {
        public int CountFrom(int city) => CountReachableInMatrix(graph, city, distances, distanceThreshold);
    }

    // The tie-break is the part of this problem that is easy to get subtly
    // wrong, so both strategies share the one scan rather than each carrying a
    // copy of the <= that makes the largest id win.
    private static int PickCityWithFewestNeighbors(int cityCount, IReachableCityCount countReachable)
    {
        var bestCity = LeetCodeAnswer.None;
        var bestCount = int.MaxValue;

        for (var city = 0; city < cityCount; city++)
        {
            var count = countReachable.CountFrom(city);

            if (count <= bestCount)
            {
                bestCount = count;
                bestCity = city;
            }
        }

        return bestCity;
    }
}
