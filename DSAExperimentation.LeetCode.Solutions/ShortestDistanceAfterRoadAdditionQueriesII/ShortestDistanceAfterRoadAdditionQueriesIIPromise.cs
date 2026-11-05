namespace DSAExperimentation.LeetCode.Solutions.ShortestDistanceAfterRoadAdditionQueriesII;

// LC 3244's promise about its input: no two roads cross, so no road starts strictly inside another and
// ends strictly past it. LeetCode's judge never poses an input that breaks it. The interval set merges
// the cities a road skips into it, which is only right when every later road nests or stays clear.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class ShortestDistanceAfterRoadAdditionQueriesIIPromise
{
    public static bool IsKeptBy(int cityCount, int[][] queries)
        => !queries.Any(road => queries.Any(other => road[0] < other[0] && other[0] < road[1] && road[1] < other[1]));
}
