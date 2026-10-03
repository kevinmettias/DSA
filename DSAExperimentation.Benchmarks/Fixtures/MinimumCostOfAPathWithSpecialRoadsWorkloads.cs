namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 2662: start and target sit at opposite corners of
// the [1, coordinateUpperBound] square and every special road's two endpoints are
// drawn inside it, which is what LC 2662 requires of an input - startX <= x1, x2 <=
// targetX, and the same on y. Each road's cost is drawn from [1, costUpperBound)
// independently of how far the road reaches, so some roads are shortcuts and some
// cost more than walking.
internal static class MinimumCostOfAPathWithSpecialRoadsWorkloads
{
    public static (int[] Start, int[] Target, int[][] SpecialRoads) Build(
        int roadCount, int coordinateUpperBound, int costUpperBound, int seed)
    {
        var random = new Random(seed);
        int[] start = [1, 1];
        int[] target = [coordinateUpperBound, coordinateUpperBound];
        var specialRoads = new int[roadCount][];

        for (var i = 0; i < roadCount; i++)
        {
            specialRoads[i] = BuildRoad(random, coordinateUpperBound, costUpperBound);
        }

        return (start, target, specialRoads);
    }

    // One [x1, y1, x2, y2, cost] road, its endpoints inside the start-to-target square.
    private static int[] BuildRoad(Random random, int coordinateUpperBound, int costUpperBound)
    {
        var coordinateEndExclusive = coordinateUpperBound + 1;

        return
        [
            random.Next(1, coordinateEndExclusive), random.Next(1, coordinateEndExclusive),
            random.Next(1, coordinateEndExclusive), random.Next(1, coordinateEndExclusive),
            random.Next(1, costUpperBound),
        ];
    }
}
