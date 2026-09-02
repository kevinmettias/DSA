using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumAreaRectangle;

// LeetCode 939. Minimum Area Rectangle: the same Set<(int X, int Y)> membership
// primitive PerfectRectangleTests already uses for corner bookkeeping, applied here
// as an O(1) existence check instead. Every pair of points is tried as a candidate
// diagonal (x1,y1)-(x2,y2); an axis-aligned rectangle exists over that diagonal iff
// the other two corners (x1,y2) and (x2,y1) are also present in the input - Set.Has
// answers that in O(1) instead of a linear rescan of the points array.
public sealed class MinimumAreaRectangleTests
{
    [Fact]
    public void MinAreaRect_ExampleWithOneRectanglePlusExtraPoint_ReturnsFour()
    {
        int[][] points = [[1, 1], [1, 3], [3, 1], [3, 3], [2, 2]];

        Assert.Equal(4, MinAreaRect(points));
    }

    [Fact]
    public void MinAreaRect_TwoOverlappingRectangles_ReturnsSmallerArea()
    {
        int[][] points = [[1, 1], [1, 3], [3, 1], [3, 3], [4, 1], [4, 3]];

        Assert.Equal(2, MinAreaRect(points));
    }

    [Fact]
    public void MinAreaRect_NoAxisAlignedRectanglePossible_ReturnsZero()
    {
        int[][] points = [[1, 1], [1, 3], [3, 1]];

        Assert.Equal(0, MinAreaRect(points));
    }

    private static int MinAreaRect(int[][] points)
    {
        var seen = BuildSeenSet(points);
        return SmallestRectangleArea(points, seen);
    }

    private static Set<(int X, int Y)> BuildSeenSet(int[][] points)
    {
        var seen = new Set<(int X, int Y)>();

        foreach (var point in points)
        {
            seen.TryAdd((point[0], point[1]));
        }

        return seen;
    }

    private static int SmallestRectangleArea(int[][] points, Set<(int X, int Y)> seen)
    {
        var minArea = int.MaxValue;

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                var area = RectangleArea(points[i], points[j], seen);

                if (area is not null)
                {
                    minArea = Math.Min(minArea, area.Value);
                }
            }
        }

        return minArea == int.MaxValue ? 0 : minArea;
    }

    private static int? RectangleArea(int[] p1, int[] p2, Set<(int X, int Y)> seen)
    {
        var (x1, y1) = (p1[0], p1[1]);
        var (x2, y2) = (p2[0], p2[1]);

        if (x1 == x2 || y1 == y2)
        {
            return null;
        }

        if (seen.Has((x1, y2)) && seen.Has((x2, y1)))
        {
            return Math.Abs((x2 - x1) * (y2 - y1));
        }

        return null;
    }
}
