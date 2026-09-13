using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumScoreTriangulationOfPolygon;

// LeetCode 1039. Minimum Score Triangulation of Polygon: interval DP over (left,
// right) vertex-index boundary pairs - for each pair, the last triangle chosen for
// that sub-polygon fixes an apex k strictly between them, whose two sides recurse
// independently into (left, k) and (k, right).
//
// This is BurstBalloonsSolution's exact (Left, Right)-keyed recurrence with min
// replacing max and no padding sentinel - a polygon has no "outside" balloon, so the
// vertex array is used as LeetCode hands it over and there is nothing to pad. Both
// strategies walk the same recurrence over the same values; they differ only in
// whether repeated (left, right) sub-polygons are cached.
internal static class MinimumScoreTriangulationOfPolygonSolution
{
    private const int MinSpanForTriangle = 2;

    // The textbook answer: plain exponential recursion over (left, right) pairs, no
    // caching - the same sub-polygon recurs across many different choices of apex
    // outside it. Deliberately written without this repo's primitives; it is the arm
    // the composed solution below has to justify itself against.
    public static int MinScoreTriangulationByUnmemoizedRecursion(int[] values) =>
        ScoreBetweenUnmemoized(0, values.Length - 1, values);

    private static int ScoreBetweenUnmemoized(int left, int right, int[] values)
    {
        if (right - left < MinSpanForTriangle)
        {
            return 0;
        }

        var best = int.MaxValue;

        for (var apex = left + 1; apex < right; apex++)
        {
            var triangleScore = (values[left] * values[apex] * values[right])
                + ScoreBetweenUnmemoized(left, apex, values) + ScoreBetweenUnmemoized(apex, right, values);
            best = Math.Min(best, triangleScore);
        }

        return best;
    }

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed by the
    // (left, right) boundary pair - the same 2-tuple-state shape BurstBalloonsSolution
    // and EditDistanceBenchmarks already use.
    public static int MinScoreTriangulationByMemoizedRecursion(int[] values) =>
        Memoizer.Memoize<(int Left, int Right), int>(
            (0, values.Length - 1),
            (range, score) => ScoreBetweenMemoized(range, score, values));

    private static int ScoreBetweenMemoized(
        (int Left, int Right) range, Func<(int Left, int Right), int> score, int[] values)
    {
        var (left, right) = range;

        if (right - left < MinSpanForTriangle)
        {
            return 0;
        }

        var best = int.MaxValue;

        for (var apex = left + 1; apex < right; apex++)
        {
            var triangleScore = (values[left] * values[apex] * values[right])
                + score((left, apex)) + score((apex, right));
            best = Math.Min(best, triangleScore);
        }

        return best;
    }
}
