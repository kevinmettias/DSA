using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.MaxValueOfEquation;

// LeetCode 1499. Max Value of Equation: the maximum of yi + yj + |xi - xj| over
// every pair of points with |xi - xj| <= k. Points arrive sorted by x and all x are
// distinct, so for i < j the target collapses to (yi - xi) + (yj + xj).
//
// FindMaxValueOfEquationByAllPairsScan is the textbook O(n^2) answer: for each i,
// walk forward while the x-distance stays within k. FindMaxValueOfEquationByMonotonicDeque
// is the O(n) reduction - the same SlidingWindowMaximum shape, this repo's own
// Deque<int> holding indices in decreasing (y - x) order, evicting from the front
// once xj - xi exceeds k and from the back once a later point's (y - x) dominates -
// keyed on an x-distance window instead of a fixed index-count window.
internal static class MaxValueOfEquationSolution
{
    // The textbook baseline: score every in-window pair directly, breaking out of
    // the inner walk as soon as the x-distance exceeds k (the points are sorted by
    // x, so nothing further along can be closer). BCL-only by design.
    public static int FindMaxValueOfEquationByAllPairsScan(int[][] points, int k)
    {
        var best = int.MinValue;

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                if (points[j][0] - points[i][0] > k)
                {
                    break;
                }

                var value = points[i][1] + points[j][1] + points[j][0] - points[i][0];
                best = Math.Max(best, value);
            }
        }

        return best;
    }

    public static int FindMaxValueOfEquationByMonotonicDeque(int[][] points, int k)
    {
        var window = new RepoDeque();
        var best = int.MinValue;

        for (var j = 0; j < points.Length; j++)
        {
            best = ScorePoint(points, k, window, j, best);
        }

        return best;
    }

    // Evicts out-of-window (x-distance > k) front entries, scores the pair against
    // the best remaining front index, then evicts back entries dominated by the new
    // point's (y - x) before pushing it - one sliding-window step.
    private static int ScorePoint(int[][] points, int k, RepoDeque window, int j, int best)
    {
        var (x, y) = (points[j][0], points[j][1]);

        while (window.TryPeekFront(out var frontIndex) && x - points[frontIndex][0] > k)
        {
            window.TryPopFront(out _);
        }

        if (window.TryPeekFront(out var bestIndex))
        {
            var value = x + y + points[bestIndex][1] - points[bestIndex][0];
            best = Math.Max(best, value);
        }

        while (window.TryPeekBack(out var backIndex) && points[backIndex][1] - points[backIndex][0] <= y - x)
        {
            window.TryPopBack(out _);
        }

        window.PushBack(j);

        return best;
    }
}
