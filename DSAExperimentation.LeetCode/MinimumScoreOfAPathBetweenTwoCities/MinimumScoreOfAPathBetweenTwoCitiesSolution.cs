using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.MinimumScoreOfAPathBetweenTwoCities;

// LeetCode 2492. Minimum Score of a Path Between Two Cities: a path may revisit
// roads and cities freely, so it can detour to any road in city 1's connected
// component and come back. The minimum score between city 1 and city n is therefore
// just the minimum weight among ALL roads in that component - the problem guarantees
// city n is in it - and roads in unrelated components must be ignored.
//
// LeetCode states each road as [from, to, distance] and numbers cities from 1, which
// is why every array below is sized cityCount + 1 and indexed from 1.
//
// Both strategies compute that same minimum; they differ only in how "is this road in
// city 1's component?" is answered: reach the component by flooding it, or label
// every city with a component root first.
internal static class MinimumScoreOfAPathBetweenTwoCitiesSolution
{
    // Positions inside LeetCode's [from, to, distance] road triple.
    private const int From = 0;
    private const int To = 1;
    private const int Distance = 2;

    // The textbook answer: build an adjacency list, flood-fill from city 1 with an
    // explicit queue and a visited array, and take the minimum weight over every edge
    // the flood touches. Deliberately written without this repo's primitives - it is
    // the arm the union-find pass has to justify itself against.
    public static int MinScoreByBreadthFirstFloodFill(int cityCount, int[][] roads)
    {
        var adjacency = BuildAdjacency(cityCount, roads);
        var flood = new CityFlood(new bool[cityCount + 1], new Queue<int>());
        flood.Reach(1);
        var minScore = int.MaxValue;

        while (flood.Pending.Count > 0)
        {
            var city = flood.Pending.Dequeue();
            var cheapestRoadOut = LeaveCity(city, adjacency, flood);
            minScore = Math.Min(minScore, cheapestRoadOut);
        }

        return minScore;
    }

    private static List<(int Neighbor, int Weight)>[] BuildAdjacency(int cityCount, int[][] roads)
    {
        var adjacency = new List<(int Neighbor, int Weight)>[cityCount + 1];

        for (var city = 1; city <= cityCount; city++)
        {
            adjacency[city] = [];
        }

        foreach (var road in roads)
        {
            adjacency[road[From]].Add((road[To], road[Distance]));
            adjacency[road[To]].Add((road[From], road[Distance]));
        }

        return adjacency;
    }

    // Every road out of a reached city counts towards the answer, whether or not it
    // leads anywhere new, because the walk may take it and come straight back.
    private static int LeaveCity(int city, List<(int Neighbor, int Weight)>[] adjacency, CityFlood flood)
    {
        var minScore = int.MaxValue;

        foreach (var (neighbor, weight) in adjacency[city])
        {
            minScore = Math.Min(minScore, weight);
            flood.Reach(neighbor);
        }

        return minScore;
    }

    // A city joins the queue the first time it is reached; the visited flag is what
    // keeps the flood finite.
    private readonly record struct CityFlood(bool[] Visited, Queue<int> Pending)
    {
        public void Reach(int city)
        {
            if (!Visited[city])
            {
                Visited[city] = true;
                Pending.Enqueue(city);
            }
        }
    }

    // This repo's own DisjointSet unions every road's endpoints - the same
    // Union-Find-for-component-membership move NumberOfProvinces and
    // GraphConnectivityWithThreshold use - after which one more pass over the roads
    // keeps the minimum weight among only those whose endpoint shares city 1's root.
    // No traversal and no adjacency list: membership is a Find, so an unrelated
    // component's cheaper road is rejected by a root comparison.
    public static int MinScoreByDisjointSet(int cityCount, int[][] roads)
    {
        var components = new DisjointSet(cityCount + 1);

        foreach (var road in roads)
        {
            components.Union(road[From], road[To]);
        }

        var targetRoot = components.Find(1);
        var minScore = int.MaxValue;

        foreach (var road in roads)
        {
            if (components.Find(road[From]) == targetRoot)
            {
                minScore = Math.Min(minScore, road[Distance]);
            }
        }

        return minScore;
    }
}
