using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DetectSquares;

// LeetCode 2013. Detect Squares: points are grouped by x-coordinate into a nested
// HashMap<int, HashMap<int,int>> (x -> y -> occurrence count) - this
// repo's own HashMap composed inside itself, the same nested-map technique
// EvaluateDivisionTests already uses for a different problem's adjacency
// structure. Count(x,y) only ever scans the points that already share the query's
// x-coordinate: for each such point (x,y2), the square's side length is |y2-y|,
// which pins the other two corners' x-coordinates to x-side and x+side, so each
// candidate square is checked (not searched for) via two more HashMap lookups.
public sealed partial class DetectSquaresTests
{
    [Fact]
    public void CountSquares_LeetCodeExample_MatchesExpectedCountsAfterEachQuery()
    {
        var detector = new Solution();
        detector.Add(3, 10);
        detector.Add(11, 2);
        detector.Add(3, 2);

        var firstCount = detector.Count(11, 10);
        Assert.Equal(1, firstCount);

        var secondCount = detector.Count(14, 8);
        Assert.Equal(0, secondCount);

        detector.Add(11, 2);

        var thirdCount = detector.Count(11, 10);
        Assert.Equal(2, thirdCount);
    }

    [Fact]
    public void CountSquares_NoPointsShareTheQueryXCoordinate_ReturnsZero()
    {
        var detector = new Solution();
        detector.Add(0, 0);

        var count = detector.Count(5, 5);
        Assert.Equal(0, count);
    }

    private sealed class Solution
    {
        private readonly HashMap<int, HashMap<int, int>> _countsByX = new();

        public void Add(int x, int y)
        {
            if (!_countsByX.TryGetValue(x, out var byY))
            {
                byY = new HashMap<int, int>();
                _countsByX.Set(x, byY);
            }

            byY.TryGetValue(y, out var existing);
            byY.Set(y, existing + 1);
        }

        public int Count(int x, int y)
        {
            if (!_countsByX.TryGetValue(x, out var sameX))
            {
                return 0;
            }

            var total = 0;

            foreach (var y2 in sameX.Keys)
            {
                if (y2 == y)
                {
                    continue;
                }

                sameX.TryGetValue(y2, out var countY2);
                var side = Math.Abs(y2 - y);

                total += CountCorner(x + side, y, y2, countY2);
                total += CountCorner(x - side, y, y2, countY2);
            }

            return total;
        }

        private int CountCorner(int otherX, int y, int y2, int countY2)
        {
            if (!_countsByX.TryGetValue(otherX, out var byY))
            {
                return 0;
            }

            byY.TryGetValue(y, out var countXY);
            byY.TryGetValue(y2, out var countXY2);

            return countXY * countXY2 * countY2;
        }
    }
}
