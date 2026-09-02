using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaxPointsOnALine;

// LeetCode 149. Max Points on a Line: anchor on each point in turn and group
// every other point by its reduced-fraction slope (dy/dx, GCD-reduced and
// sign-normalized) in this repo's own HashMap<(int,int),int> - the largest
// slope bucket at any anchor, plus the anchor itself, is the answer. Avoids
// the O(n^3) every-triple-of-points brute force.
public sealed class MaxPointsOnALineTests
{
    [Fact]
    public void MaxPoints_ThreeCollinearPoints_ReturnsThree()
    {
        int[][] points = [[1, 1], [2, 2], [3, 3]];

        Assert.Equal(3, MaxPoints(points));
    }

    [Fact]
    public void MaxPoints_FourOfSixPointsCollinear_ReturnsFour()
    {
        int[][] points = [[1, 1], [3, 2], [5, 3], [4, 1], [2, 3], [1, 4]];

        Assert.Equal(4, MaxPoints(points));
    }

    [Fact]
    public void MaxPoints_DuplicatePointsOnSameLine_CountsEachPoint()
    {
        int[][] points = [[0, 0], [0, 0], [1, 1], [2, 2]];

        Assert.Equal(4, MaxPoints(points));
    }

    private static int MaxPoints(int[][] points)
    {
        if (points.Length <= 2)
        {
            return points.Length;
        }

        var best = 1;

        for (var i = 0; i < points.Length; i++)
        {
            best = Math.Max(best, ScanFromAnchor(points, i) + 1);
        }

        return best;
    }

    private static int ScanFromAnchor(int[][] points, int i)
    {
        var scan = new AnchorScan();

        for (var j = 0; j < points.Length; j++)
        {
            if (j == i)
            {
                continue;
            }

            scan.Consider(points, i, j);
        }

        return scan.Result;
    }

    private sealed class AnchorScan
    {
        private readonly HashMap<(int Dx, int Dy), int> _slopeCounts = new();
        private int _duplicates;
        private int _localBest;

        public int Result => _localBest + _duplicates;

        public void Consider(int[][] points, int i, int j)
        {
            var dx = points[j][0] - points[i][0];
            var dy = points[j][1] - points[i][1];

            if (dx == 0 && dy == 0)
            {
                _duplicates++;
                return;
            }

            var key = ReducedSlope(dx, dy);
            var count = _slopeCounts.TryGetValue(key, out var existing) ? existing + 1 : 1;
            _slopeCounts.Set(key, count);
            _localBest = Math.Max(_localBest, count);
        }
    }

    // Reduces (dx, dy) to a canonical slope key: divide out the GCD, then fix
    // the sign so (dx, dy) and (-dx, -dy) - the same line, opposite direction -
    // always hash to the same bucket.
    private static (int, int) ReducedSlope(int dx, int dy)
    {
        var divisor = Gcd(Math.Abs(dx), Math.Abs(dy));
        dx /= divisor;
        dy /= divisor;

        if (dx < 0 || (dx == 0 && dy < 0))
        {
            dx = -dx;
            dy = -dy;
        }

        return (dx, dy);
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
