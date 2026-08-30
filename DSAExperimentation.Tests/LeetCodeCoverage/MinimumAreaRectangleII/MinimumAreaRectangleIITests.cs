using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumAreaRectangleII;

// LeetCode 963. Minimum Area Rectangle II: unlike LC 939's axis-aligned version
// (MinimumAreaRectangleTests, a Set<(int,int)> corner-membership check), a
// rectangle here can be rotated to any angle, so "the other two corners" isn't a
// fixed offset to look up. Instead: a quadrilateral is a rectangle iff its two
// diagonals bisect each other AND are equal length (textbook theorem), so every
// point-pair is tried as a candidate diagonal and grouped by
// (2*midpointX, 2*midpointY, lengthSquared) in this repo's own
// HashMap<TKey,TValue> - the same "hash the derived key, not the raw pair" move
// TwoSumBenchmarks/MinimumAreaRectangleBenchmarks already make. Two pairs sharing
// a key are the two diagonals of one rectangle; the shared diagonal's own two
// endpoints are then each other's adjacent (and, by the theorem, automatically
// perpendicular) sides, so area is just the product of those two side lengths -
// no separate right-angle check needed.
public sealed class MinimumAreaRectangleIITests
{
    [Fact]
    public void MinAreaFreeRect_RotatedSquareExample_ReturnsTwo()
    {
        int[][] points = [[1, 2], [2, 1], [1, 0], [0, 1]];

        Assert.Equal(2.0, MinAreaFreeRect(points), precision: 5);
    }

    [Fact]
    public void MinAreaFreeRect_AxisAlignedRectangleAmongExtraPoints_ReturnsTwelve()
    {
        int[][] points = [[0, 0], [4, 0], [4, 3], [0, 3], [2, 2], [5, 5]];

        Assert.Equal(12.0, MinAreaFreeRect(points), precision: 5);
    }

    [Fact]
    public void MinAreaFreeRect_FewerThanFourPoints_ReturnsZero()
    {
        int[][] points = [[1, 1], [2, 2], [3, 3]];

        Assert.Equal(0.0, MinAreaFreeRect(points), precision: 5);
    }

    private static double MinAreaFreeRect(int[][] points)
    {
        var diagonalsByKey =
            new HashMap<(int SumX, int SumY, int LengthSquared), List<((int X, int Y) First, (int X, int Y) Second)>>();
        var minArea = double.MaxValue;

        for (var i = 0; i < points.Length; i++)
        {
            var pointI = (X: points[i][0], Y: points[i][1]);

            for (var j = i + 1; j < points.Length; j++)
            {
                var pointJ = (X: points[j][0], Y: points[j][1]);
                var dx = pointI.X - pointJ.X;
                var dy = pointI.Y - pointJ.Y;
                var key = (pointI.X + pointJ.X, pointI.Y + pointJ.Y, (dx * dx) + (dy * dy));

                if (!diagonalsByKey.TryGetValue(key, out var matchingDiagonals))
                {
                    matchingDiagonals = [];
                    diagonalsByKey.Set(key, matchingDiagonals);
                }

                foreach (var (first, second) in matchingDiagonals)
                {
                    var sideA = Distance(pointI, first);
                    var sideB = Distance(pointI, second);
                    minArea = Math.Min(minArea, sideA * sideB);
                }

                matchingDiagonals.Add((pointI, pointJ));
            }
        }

        return minArea == double.MaxValue ? 0.0 : minArea;
    }

    private static double Distance((int X, int Y) a, (int X, int Y) b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return Math.Sqrt((dx * dx) + (dy * dy));
    }
}
