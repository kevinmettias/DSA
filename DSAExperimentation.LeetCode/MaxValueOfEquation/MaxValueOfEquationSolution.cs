using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.MaxValueOfEquation;

// LeetCode 1499. Max Value of Equation: the maximum of yi + yj + |xi - xj| over
// every pair of points with |xi - xj| <= maxDistance. Points arrive sorted by x and
// all x are distinct, so for i < j the target collapses to (yi - xi) + (yj + xj).
//
// FindMaxValueOfEquationByAllPairsScan is the textbook O(n^2) answer: for each i,
// walk forward while the x-distance stays within maxDistance.
// FindMaxValueOfEquationByMonotonicDeque is the O(n) reduction - the same
// SlidingWindowMaximum shape, this repo's own Deque<int> holding indices in
// decreasing (y - x) order, evicting from the front once xj - xi exceeds maxDistance
// and from the back once a later point's (y - x) dominates - keyed on an
// x-distance window instead of a fixed index-count window.
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
        var window = new RepoDeque();
        var best = int.MinValue;

        for (var j = 0; j < points.Length; j++)
        {
            best = ScorePoint((points, maxDistance), window, j, best);
        }

        return best;
    }

    // Evicts out-of-window (x-distance > maxDistance) front entries, scores the pair
    // against the best remaining front index, then evicts back entries dominated by the
    // new point's (y - x) before pushing it - one sliding-window step. The points and the
    // maxDistance that bounds every pair of them are the query both strategies are asked,
    // so they travel as one argument.
    private static int ScorePoint((int[][] Points, int MaxDistance) query, RepoDeque window, int pointIndex, int best)
    {
        best = BestAgainstFront(query, window, pointIndex, best);
        InsertIntoWindow(query, window, pointIndex);

        return best;
    }

    // Evicts out-of-window (x-distance > maxDistance) front entries, then scores the pair
    // against the best front index that survived.
    private static int BestAgainstFront((int[][] Points, int MaxDistance) query, RepoDeque window, int pointIndex, int best)
    {
        var (x, y) = (query.Points[pointIndex][0], query.Points[pointIndex][1]);

        while (window.TryPeekFront(out var frontIndex) && x - query.Points[frontIndex][0] > query.MaxDistance)
        {
            window.TryPopFront(out _);
        }

        if (window.TryPeekFront(out var bestIndex))
        {
            var value = x + y + query.Points[bestIndex][1] - query.Points[bestIndex][0];
            best = Math.Max(best, value);
        }

        return best;
    }

    // Evicts the back entries the new point's (y - x) dominates, then pushes it, so the
    // window keeps its decreasing-(y - x) order for the next step.
    private static void InsertIntoWindow((int[][] Points, int MaxDistance) query, RepoDeque window, int pointIndex)
    {
        var (x, y) = (query.Points[pointIndex][0], query.Points[pointIndex][1]);

        while (window.TryPeekBack(out var backIndex) && query.Points[backIndex][1] - query.Points[backIndex][0] <= y - x)
        {
            window.TryPopBack(out _);
        }

        window.PushBack(pointIndex);
    }
}
