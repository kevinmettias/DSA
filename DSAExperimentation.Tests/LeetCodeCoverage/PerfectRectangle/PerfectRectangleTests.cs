using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PerfectRectangle;

// LeetCode 391. Perfect Rectangle: a total-area-vs-bounding-box check (catches gaps)
// plus a Set<(int X, int Y)> corner-toggle pass (catches overlaps) - only the 4 outer
// corners of a genuine perfect cover survive an odd number of toggles, since every
// interior corner is always shared by an even number of rectangles. Set<T> (this
// repo's own composition of HashMap<T,bool>) is exactly the membership-toggle
// primitive this needs: TryAdd/TryRemove already give add-if-absent/remove-if-present
// for free.
public sealed class PerfectRectangleTests
{
    [Fact]
    public void IsRectangleCover_ExactTiling_ReturnsTrue()
    {
        int[][] rectangles =
        [
            [1, 1, 3, 3],
            [3, 1, 4, 2],
            [3, 2, 4, 4],
            [1, 3, 2, 4],
            [2, 3, 3, 4],
        ];

        Assert.True(IsRectangleCover(rectangles));
    }

    [Fact]
    public void IsRectangleCover_GapBetweenRegions_ReturnsFalse()
    {
        // LeetCode's own second example: total area (6) falls short of the bounding
        // box's area (9), so the area check alone catches this one.
        int[][] rectangles =
        [
            [1, 1, 2, 3],
            [1, 3, 2, 4],
            [3, 1, 4, 2],
            [3, 2, 4, 4],
        ];

        Assert.False(IsRectangleCover(rectangles));
    }

    [Fact]
    public void IsRectangleCover_OverlapMaskedByEqualSizedGap_ReturnsFalse()
    {
        // A deliberately harder case than the gap example above: an overlapping
        // region (x in [1,2], y in [2,4]) is offset by an equal-area gap elsewhere
        // (x in [3,4], y in [2,4]), so total area (16) still equals the 4x4 bounding
        // box's area (16) - the area check alone cannot catch this, only the corner
        // count (10 survivors here, not 4) can.
        int[][] rectangles =
        [
            [0, 0, 2, 4],
            [2, 0, 4, 2],
            [1, 2, 3, 4],
        ];

        Assert.False(IsRectangleCover(rectangles));
    }

    private static bool IsRectangleCover(int[][] rectangles)
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;
        long totalArea = 0;
        var corners = new Set<(int X, int Y)>();

        foreach (var rect in rectangles)
        {
            var (x1, y1, x2, y2) = (rect[0], rect[1], rect[2], rect[3]);
            minX = Math.Min(minX, x1);
            minY = Math.Min(minY, y1);
            maxX = Math.Max(maxX, x2);
            maxY = Math.Max(maxY, y2);
            totalArea += (long)(x2 - x1) * (y2 - y1);

            ToggleCorner(corners, (x1, y1));
            ToggleCorner(corners, (x1, y2));
            ToggleCorner(corners, (x2, y1));
            ToggleCorner(corners, (x2, y2));
        }

        if (totalArea != (long)(maxX - minX) * (maxY - minY) || corners.Count != 4)
        {
            return false;
        }

        return corners.Has((minX, minY)) && corners.Has((minX, maxY))
            && corners.Has((maxX, minY)) && corners.Has((maxX, maxY));
    }

    private static void ToggleCorner(Set<(int X, int Y)> corners, (int X, int Y) point)
    {
        if (!corners.TryAdd(point))
        {
            corners.TryRemove(point);
        }
    }
}
