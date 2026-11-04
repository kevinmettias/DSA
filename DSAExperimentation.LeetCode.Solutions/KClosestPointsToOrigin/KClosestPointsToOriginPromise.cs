namespace DSAExperimentation.LeetCode.Solutions.KClosestPointsToOrigin;

// LC 973's promise about its input: the answer is unique apart from its order, so the kth closest
// point is strictly closer than the next. LeetCode's judge never poses an input that breaks it.
// Given a tie at the kth distance, the full sort and the heap keep different points. Generated
// cases do not always keep the promise, so a harness that runs them asks this first.
internal static class KClosestPointsToOriginPromise
{
    public static bool IsKeptBy(int[][] points, int nearestCount)
    {
        var distances = points.Select(SquaredDistance).Order().ToArray();

        return nearestCount == distances.Length || distances[nearestCount - 1] < distances[nearestCount];
    }

    private static long SquaredDistance(int[] point) => ((long)point[0] * point[0]) + ((long)point[1] * point[1]);
}
