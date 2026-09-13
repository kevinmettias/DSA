namespace DSAExperimentation.Benchmarks.Fixtures;

// Workload sizing only (ARCHITECTURE.md 17.7): builds LC 1334's own
// int[][] { from, to, weight } road list for a connected, undirected,
// non-negative-weight network - every city i > 0 gets a road back to some
// earlier city j < i (guaranteeing the graph is connected), plus a few extra
// random roads per city for density, so neither shortest-path strategy is
// measured on a degenerate tree. RandomWeightedGraphs is the directed sibling
// of this generator and builds nodes rather than LeetCode's edge array, so it
// cannot serve here.
internal static class CityRoadWorkloads
{
    // Exclusive upper bound passed to Random.Next(1, _): road weights land in [1, 19].
    private const int RoadWeightUpperBound = 20;

    public static int[][] BuildRoads(int cityCount, int extraRoadsPerCity, int seed)
    {
        var random = new Random(seed);
        var roads = new List<int[]>();

        AddBackRoads(roads, cityCount, random);
        AddExtraRoads(roads, cityCount, extraRoadsPerCity, random);

        return [.. roads];
    }

    private static void AddBackRoads(List<int[]> roads, int cityCount, Random random)
    {
        for (var i = 1; i < cityCount; i++)
        {
            var j = random.Next(i);

            roads.Add([i, j, random.Next(1, RoadWeightUpperBound)]);
        }
    }

    private static void AddExtraRoads(List<int[]> roads, int cityCount, int extraRoadsPerCity, Random random)
    {
        for (var i = 0; i < cityCount; i++)
        {
            for (var e = 0; e < extraRoadsPerCity; e++)
            {
                var target = random.Next(cityCount);

                if (target != i)
                {
                    roads.Add([i, target, random.Next(1, RoadWeightUpperBound)]);
                }
            }
        }
    }
}
