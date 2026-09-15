using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.MinimumAreaRectangleII;

// LeetCode 963. Minimum Area Rectangle II: the smallest area of a rectangle whose
// four corners are all in the given point set, or 0 if none exists. Unlike LC 939's
// axis-aligned version (MinimumAreaRectangleSolution, a Set<(int,int)> corner
// lookup), a rectangle here may be rotated to any angle, so "the other two corners"
// is not a fixed offset to look up.
//
// Both strategies rest on the same textbook theorem: a quadrilateral is a rectangle
// iff its two diagonals bisect each other AND have equal length. Given the two
// diagonals, the corners adjacent to either diagonal endpoint are automatically
// perpendicular sides, so the area is just the product of the two side lengths
// meeting there - no separate right-angle check is needed.
//
// MinAreaFreeRectByQuadrupleScan is the naive baseline: every 4-point combination
// (O(n^4)), each tried under all 3 ways of splitting those 4 points into two
// candidate diagonals, with the bisect-and-equal-length criterion checked by hand.
// Written with nothing but BCL arrays.
//
// MinAreaFreeRectByDiagonalGrouping tries every point PAIR once (O(n^2)) as a
// candidate diagonal and groups the pairs by (2*midpointX, 2*midpointY,
// lengthSquared) in this repo's own HashMap<TKey,TValue> - any two pairs colliding
// on that derived key are already known to be one rectangle's two diagonals. It is
// the same "hash the derived key instead of rechecking the geometry" move
// MinimumAreaRectangleSolution's set lookup and TwoSumSolution's complement lookup
// both make.
internal static class MinimumAreaRectangleIISolution
{
    private const double NoRectangle = 0.0;

    public static double MinAreaFreeRectByQuadrupleScan(int[][] points)
    {
        var minArea = double.MaxValue;

        for (var a = 0; a < points.Length; a++)
        {
            for (var b = a + 1; b < points.Length; b++)
            {
                for (var c = b + 1; c < points.Length; c++)
                {
                    var areaForTriple = BestAreaForTriple(points, a, b, c);
                    minArea = Math.Min(minArea, areaForTriple);
                }
            }
        }

        return minArea == double.MaxValue ? NoRectangle : minArea;
    }

    // The fourth corner's loop, split out so the quadruple scan itself stays three
    // levels deep. Each d yields the three distinct diagonal pairings of {a,b,c,d}.
    private static double BestAreaForTriple(int[][] points, int a, int b, int c)
    {
        var minArea = double.MaxValue;

        for (var d = c + 1; d < points.Length; d++)
        {
            var areaWithDiagonalsAbCd = RectangleArea(points, new DiagonalCandidate(a, c, b, d)); // diagonals (a,b), (c,d)
            minArea = Smaller(minArea, areaWithDiagonalsAbCd);

            var areaWithDiagonalsAcBd = RectangleArea(points, new DiagonalCandidate(a, b, c, d)); // diagonals (a,c), (b,d)
            minArea = Smaller(minArea, areaWithDiagonalsAcBd);

            var areaWithDiagonalsAdBc = RectangleArea(points, new DiagonalCandidate(a, b, d, c)); // diagonals (a,d), (b,c)
            minArea = Smaller(minArea, areaWithDiagonalsAdBc);
        }

        return minArea;
    }

    private static double? RectangleArea(int[][] points, DiagonalCandidate candidate)
    {
        var (first, second, third, fourth) = candidate;
        var cornerOne = Point(points, first);
        var cornerTwo = Point(points, second);
        var cornerThree = Point(points, third);
        var cornerFour = Point(points, fourth);

        if (cornerOne.X + cornerThree.X != cornerTwo.X + cornerFour.X ||
            cornerOne.Y + cornerThree.Y != cornerTwo.Y + cornerFour.Y)
        {
            return null;
        }

        if (LengthSquared(cornerOne, cornerThree) != LengthSquared(cornerTwo, cornerFour))
        {
            return null;
        }

        return Distance(cornerOne, cornerTwo) * Distance(cornerOne, cornerFour);
    }

    private static double Distance((int X, int Y) a, (int X, int Y) b)
    {
        var lengthSquared = LengthSquared(a, b);
        return Math.Sqrt(lengthSquared);
    }

    public static double MinAreaFreeRectByDiagonalGrouping(int[][] points)
    {
        var index = new DiagonalIndex();
        var minArea = double.MaxValue;

        for (var i = 0; i < points.Length; i++)
        {
            var pointI = Point(points, i);

            for (var j = i + 1; j < points.Length; j++)
            {
                var pointJ = Point(points, j);
                var area = index.ConsiderPair(pointI, pointJ);
                minArea = Smaller(minArea, area);
            }
        }

        return minArea == double.MaxValue ? NoRectangle : minArea;
    }

    private static (int X, int Y) Point(int[][] points, int index) => (points[index][0], points[index][1]);

    private static double Smaller(double minArea, double? candidate) =>
        candidate is null ? minArea : Math.Min(minArea, candidate.Value);

    private static int LengthSquared((int X, int Y) a, (int X, int Y) b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return (dx * dx) + (dy * dy);
    }

    // Diagonal candidates are (First, Third) and (Second, Fourth) - a rectangle iff
    // the two share a midpoint and a length.
    private readonly record struct DiagonalCandidate(int First, int Second, int Third, int Fourth);

    // Every point pair seen so far, bucketed by the key that makes two pairs a
    // rectangle's diagonals: twice the midpoint, plus the squared length.
    private sealed class DiagonalIndex
    {
        private readonly HashMap<(int SumX, int SumY, int LengthSquared), List<((int X, int Y) First, (int X, int Y) Second)>> _diagonalsByKey = new();

        public double? ConsiderPair((int X, int Y) pointI, (int X, int Y) pointJ)
        {
            var matchingDiagonals = BucketFor(pointI, pointJ);
            var best = BestAreaAgainst(pointI, matchingDiagonals);
            matchingDiagonals.Add((pointI, pointJ));
            return best;
        }

        // The pairs already keyed to this pair's derived key, or a fresh empty bucket
        // registered under that key so a later pair can collide with it.
        private List<((int X, int Y) First, (int X, int Y) Second)> BucketFor(
            (int X, int Y) pointI, (int X, int Y) pointJ)
        {
            var key = (pointI.X + pointJ.X, pointI.Y + pointJ.Y, LengthSquared(pointI, pointJ));

            if (!_diagonalsByKey.TryGetValue(key, out var matchingDiagonals))
            {
                matchingDiagonals = [];
                _diagonalsByKey.Set(key, matchingDiagonals);
            }

            return matchingDiagonals;
        }

        // pointI and each stored pair's endpoints are adjacent corners of one
        // rectangle, so the area is the product of those two sides.
        private static double? BestAreaAgainst(
            (int X, int Y) pointI,
            List<((int X, int Y) First, (int X, int Y) Second)> matchingDiagonals)
        {
            double? best = null;

            foreach (var (first, second) in matchingDiagonals)
            {
                var candidate = Distance(pointI, first) * Distance(pointI, second);
                best = best is null ? candidate : Math.Min(best.Value, candidate);
            }

            return best;
        }
    }
}
