using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaxValueOfEquation;

// LeetCode 1499. Max Value of Equation: points are already sorted by x, so for i < j
// the target yi + yj + |xi - xj| collapses to (yi - xi) + (yj + xj). Scanning j left to
// right, the best i is whichever in-window index maximizes (y - x) - the same
// SlidingWindowMaximum shape (this repo's own Deque<int> holding indices in decreasing
// (y - x) order), just keyed on an x-distance window (xj - xi <= k) instead of a fixed
// index-count window.
public sealed class MaxValueOfEquationTests
{
    [Fact]
    public void FindMaxValueOfEquation_ClassicExample_ReturnsFour()
    {
        int[][] points = [[1, 3], [2, 0], [5, 10], [6, -10]];

        Assert.Equal(4, FindMaxValueOfEquation(points, k: 1));
    }

    [Fact]
    public void FindMaxValueOfEquation_WideWindow_AllowsFarApartPair()
    {
        int[][] points = [[0, 0], [3, 0], [9, 2]];

        Assert.Equal(3, FindMaxValueOfEquation(points, k: 3));
    }

    private static int FindMaxValueOfEquation(int[][] points, int k)
    {
        var window = new RepoDeque();
        var best = int.MinValue;

        for (var j = 0; j < points.Length; j++)
        {
            var (x, y) = (points[j][0], points[j][1]);

            while (window.TryPeekFront(out var frontIndex) && x - points[frontIndex][0] > k)
            {
                window.TryPopFront(out _);
            }

            if (window.TryPeekFront(out var bestIndex))
            {
                best = Math.Max(best, x + y + points[bestIndex][1] - points[bestIndex][0]);
            }

            while (window.TryPeekBack(out var backIndex) && points[backIndex][1] - points[backIndex][0] <= y - x)
            {
                window.TryPopBack(out _);
            }

            window.PushBack(j);
        }

        return best;
    }
}
