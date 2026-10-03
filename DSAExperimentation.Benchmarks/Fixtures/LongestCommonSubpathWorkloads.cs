namespace DSAExperimentation.Benchmarks.Fixtures;

// Workload sizing for LC 1923: friends' paths over a deliberately tiny set of cities,
// so long shared runs really do occur and the binary search has to climb rather than
// bail at length 1. Each step moves a nonzero distance around the CityCount cities,
// so no path lists the same city twice in a row, as LC 1923 promises.
internal static class LongestCommonSubpathWorkloads
{
    private const int CityCount = 5;

    public static int[][] BuildPaths(int pathCount, int pathLength, int seed)
    {
        var random = new Random(seed);
        var paths = new int[pathCount][];

        for (var i = 0; i < pathCount; i++)
        {
            paths[i] = BuildPath(pathLength, random);
        }

        return paths;
    }

    // Cities are numbered 1..CityCount; a step of 1..CityCount - 1 around them never lands
    // back where it started.
    private static int[] BuildPath(int pathLength, Random random)
    {
        var path = new int[pathLength];
        path[0] = random.Next(1, CityCount + 1);

        for (var j = 1; j < pathLength; j++)
        {
            var step = random.Next(1, CityCount);
            path[j] = ((path[j - 1] - 1 + step) % CityCount) + 1;
        }

        return path;
    }
}
