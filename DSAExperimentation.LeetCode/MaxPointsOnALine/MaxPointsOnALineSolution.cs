using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.MaxPointsOnALine;

// LeetCode 149. Max Points on a Line: the size of the largest subset of points
// that lie on some common straight line.
//
// MaxPointsByCrossProduct is the textbook O(n^3) check: for every pair of
// points, count every third point whose cross product against that pair is
// zero. MaxPointsBySlopeGrouping anchors on each point in turn and groups
// every other point by its reduced-fraction slope (dy/dx, GCD-reduced and
// sign-normalized) in this repo's own HashMap<SlopeKey,int> - the largest
// slope bucket at any anchor, plus the anchor itself and any point that
// coincides with it (which lies on every line through the anchor, not just
// one slope's bucket), is that anchor's best line.
internal static class MaxPointsOnALineSolution
{
    private const int TrivialPointCount = 2;

    public static int MaxPointsByCrossProduct(int[][] points)
    {
        var n = points.Length;

        if (n <= TrivialPointCount)
        {
            return n;
        }

        var best = TrivialPointCount;

        for (var i = 0; i < n; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                var collinearWithPair = CountCollinearWithPair(points, i, j);
                best = Math.Max(best, collinearWithPair);
            }
        }

        return best;
    }

    private static int CountCollinearWithPair(int[][] points, int i, int j)
    {
        var count = TrivialPointCount;

        for (var k = j + 1; k < points.Length; k++)
        {
            var cross = (long)(points[j][0] - points[i][0]) * (points[k][1] - points[i][1])
                      - (long)(points[j][1] - points[i][1]) * (points[k][0] - points[i][0]);

            if (cross == 0)
            {
                count++;
            }
        }

        return count;
    }

    public static int MaxPointsBySlopeGrouping(int[][] points)
    {
        if (points.Length <= TrivialPointCount)
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

    private static int ScanFromAnchor(int[][] points, int anchor)
    {
        var scan = new AnchorScan();

        for (var j = 0; j < points.Length; j++)
        {
            if (j != anchor)
            {
                scan.Consider(points, anchor, j);
            }
        }

        return scan.Result;
    }

    // Tracks, for one anchor, the largest count of other points sharing a
    // reduced slope, plus a separate tally of points that coincide with the
    // anchor - those lie on every line through it, so they add to whichever
    // slope wins rather than forming a "slope" bucket of their own.
    private sealed class AnchorScan
    {
        private readonly HashMap<SlopeKey, int> _slopeCounts = new();
        private int _duplicates;
        private int _localBest;

        public int Result => _localBest + _duplicates;

        public void Consider(int[][] points, int anchor, int other)
        {
            var dx = points[other][0] - points[anchor][0];
            var dy = points[other][1] - points[anchor][1];

            if (dx == 0 && dy == 0)
            {
                _duplicates++;
                return;
            }

            var key = ReducedSlope(dx, dy);
            var count = CountIncludingThisPoint(key);
            _slopeCounts.Set(key, count);
            _localBest = Math.Max(_localBest, count);
        }

        // How many points share this slope with the anchor now: one more than the
        // count this point joins, or the 1 that starts the slope when it is the
        // first point to take it.
        private int CountIncludingThisPoint(SlopeKey key)
        {
            if (_slopeCounts.TryGetValue(key, out var existing))
            {
                return existing + 1;
            }

            return 1;
        }
    }

    // Reduces (dx, dy) to a canonical slope key: divide out the GCD, then fix
    // the sign so (dx, dy) and (-dx, -dy) - the same line, opposite direction -
    // always hash to the same bucket.
    private static SlopeKey ReducedSlope(int dx, int dy)
    {
        var divisor = Gcd(Math.Abs(dx), Math.Abs(dy));
        dx /= divisor;
        dy /= divisor;

        if (PointsTheWrongWay(dx, dy))
        {
            dx = -dx;
            dy = -dy;
        }

        return new(dx, dy);
    }

    // A direction pointing left, or straight down, is the flipped twin of one that
    // does not - the same line, opposite direction - so it is the one negated.
    private static bool PointsTheWrongWay(int dx, int dy)
        => dx < 0 || (dx == 0 && dy < 0);

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
