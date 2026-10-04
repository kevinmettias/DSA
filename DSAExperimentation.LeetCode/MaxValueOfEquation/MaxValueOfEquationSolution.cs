using DSAExperimentation.DataStructures.MonotonicDeque;

namespace DSAExperimentation.LeetCode.MaxValueOfEquation;

// LeetCode 1499. Max Value of Equation: the maximum of yi + yj + |xi - xj| over
// every pair of points with |xi - xj| <= maxDistance. Points arrive sorted by x and
// all x are distinct, so for i < j the target collapses to (yi - xi) + (yj + xj).
//
// FindMaxValueOfEquationByAllPairsScan is the textbook O(n^2) answer: for each i,
// walk forward while the x-distance stays within maxDistance.
// FindMaxValueOfEquationByMonotonicDeque is the O(n) reduction - the same
// SlidingWindowMaximum shape, this repo's MonotonicDeque under MaxWindowOrder keyed
// on (y - x), evicting from the front once xj - xi exceeds maxDistance and from the
// back once a later point's (y - x) dominates - measured in x-coordinates instead of
// a fixed index-count window.
internal static class MaxValueOfEquationSolution
{
    // The textbook baseline: score every in-window pair directly, breaking out of
    // the inner walk as soon as the x-distance exceeds maxDistance (the points are
    // sorted by x, so nothing further along can be closer). BCL-only by design.
    public static int FindMaxValueOfEquationByAllPairsScan(int[][] points, int maxDistance)
    {
        var best = int.MinValue;

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                if (points[j][0] - points[i][0] > maxDistance)
                {
                    break;
                }

                var value = points[i][1] + points[j][1] + points[j][0] - points[i][0];
                best = Math.Max(best, value);
            }
        }

        return best;
    }

    public static int FindMaxValueOfEquationByMonotonicDeque(int[][] points, int maxDistance)
    {
        var window = new MonotonicDeque<int, MaxWindowOrder<int>>();
        var best = int.MinValue;

        for (var j = 0; j < points.Length; j++)
        {
            best = BestWithWindowPartner((points, maxDistance), window, j, best);

            // The window's keys are (y - x), so pushing the point evicts every earlier
            // point whose (y - x) it matches or beats.
            window.Push(points[j][0], points[j][1] - points[j][0]);
        }

        return best;
    }

    // One sliding-window score. The window's positions are x-coordinates, which arrive
    // strictly increasing, so dropping every point more than maxDistance behind is
    // EvictBefore(x - maxDistance); its keys are (y - x), so the surviving front is the
    // best partner and the pair scores (y + x) plus that key. The points and the
    // maxDistance that bounds every pair of them are the query both strategies are
    // asked, so they travel as one argument.
    private static int BestWithWindowPartner(
        (int[][] Points, int MaxDistance) query,
        MonotonicDeque<int, MaxWindowOrder<int>> window,
        int pointIndex,
        int best)
    {
        var (x, y) = (query.Points[pointIndex][0], query.Points[pointIndex][1]);

        window.EvictBefore(x - query.MaxDistance);

        if (window.TryPeekFront(out var partner))
        {
            best = Math.Max(best, x + y + partner.Key);
        }

        return best;
    }
}
