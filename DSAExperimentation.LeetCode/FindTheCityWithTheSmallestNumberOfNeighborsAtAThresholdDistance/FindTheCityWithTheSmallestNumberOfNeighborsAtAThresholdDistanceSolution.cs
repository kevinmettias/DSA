using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistance;

// LeetCode 1334. Find the City With the Smallest Number of Neighbors at a
// Threshold Distance: given n cities joined by weighted, undirected roads,
// report the city that can reach the fewest other cities within
// distanceThreshold, breaking ties in favour of the largest city id.
//
// Both strategies answer the identical question - "for every city, how many
// other cities lie within the threshold" - and differ only in which
// shortest-path primitive they ask. That is the whole point of the pair: the
// single-source primitive most shortest-path problems in this repo reach for,
// run once per city, against the one all-pairs primitive that answers the
// question in a single call. Ties favour the larger id by scanning ascending
// and replacing the best candidate on a <=, in both arms.
internal static class FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceSolution
{
    // The obvious composition, and the arm the all-pairs strategy below has to
    // justify itself against: ShortestPath.Dijkstra from every city in turn,
    // counting that source's own reachable set straight off its distance map.
    // Unlike most baselines in this tier this one is deliberately NOT a BCL
    // rewrite (ARCHITECTURE.md 17.5) - what it exists to show is that reaching
    // for the repo's *single-source* primitive on an all-pairs question is the
    // wrong tool, which only means anything if it really is that primitive.
    public static int FindCityByDijkstraPerSource(int n, int[][] edges, int distanceThreshold) =>
        FindCityByDijkstraPerSource(CityGraph.Build(n, edges), distanceThreshold);

    public static int FindCityByDijkstraPerSource(CityGraph graph, int distanceThreshold) =>
        PickCityWithFewestNeighbors(
            graph.Cities.Length,
            city => CountReachableByDijkstra(graph.Cities[city], distanceThreshold));

    private static int CountReachableByDijkstra(CityNode source, int distanceThreshold)
    {
        var distances = ShortestPath
            .Dijkstra<CityNode, CityTopology, ListEdges<CityNode, int>, int>(source);

        return distances.Count(pair => pair.Key != source && pair.Value <= distanceThreshold);
    }

    // AllPairsShortestPaths.TryComputeDistances (Floyd-Warshall) is already
    // exactly "every pair's distance", which is what this problem asks for
    // n times over, so one call replaces the n searches above and every city's
    // count is then a lookup. Unreachable pairs are absent from the map rather
    // than carrying a sentinel, so a missing entry simply fails the threshold
    // test. The bool result reports a negative cycle, which non-negative road
    // lengths make impossible here.
    public static int FindCityByFloydWarshall(int n, int[][] edges, int distanceThreshold) =>
        FindCityByFloydWarshall(CityGraph.Build(n, edges), distanceThreshold);

    public static int FindCityByFloydWarshall(CityGraph graph, int distanceThreshold)
    {
        _ = AllPairsShortestPaths.TryComputeDistances<CityNode, CityTopology, ListEdges<CityNode, int>, int>(
            graph.Cities, out var distances);

        return PickCityWithFewestNeighbors(
            graph.Cities.Length,
            city => CountReachableInMatrix(graph, city, distances, distanceThreshold));
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
                && distances.TryGetValue((graph.Cities[city], graph.Cities[other]), out var distance)
                && distance <= distanceThreshold)
            {
                count++;
            }
        }

        return count;
    }

    // The tie-break is the part of this problem that is easy to get subtly
    // wrong, so both strategies share the one scan rather than each carrying a
    // copy of the <= that makes the largest id win.
    private static int PickCityWithFewestNeighbors(int cityCount, Func<int, int> countReachable)
    {
        var bestCity = LeetCodeAnswer.None;
        var bestCount = int.MaxValue;

        for (var city = 0; city < cityCount; city++)
        {
            var count = countReachable(city);

            if (count <= bestCount)
            {
                bestCount = count;
                bestCity = city;
            }
        }

        return bestCity;
    }
}
