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
        var accumulator = new RectangleAccumulator();

        foreach (var rect in rectangles)
        {
            accumulator.Add(rect);
        }

        return accumulator.IsPerfectCover();
    }

    private sealed class RectangleAccumulator
    {
        private readonly Set<(int X, int Y)> _corners = new();
        private int _minX = int.MaxValue;
        private int _minY = int.MaxValue;
        private int _maxX = int.MinValue;
        private int _maxY = int.MinValue;
        private long _totalArea;

        public void Add(int[] rect)
        {
            var (x1, y1, x2, y2) = (rect[0], rect[1], rect[2], rect[3]);
            _minX = Math.Min(_minX, x1);
            _minY = Math.Min(_minY, y1);
            _maxX = Math.Max(_maxX, x2);
            _maxY = Math.Max(_maxY, y2);
            _totalArea += (long)(x2 - x1) * (y2 - y1);

            ToggleCorner(_corners, (x1, y1));
            ToggleCorner(_corners, (x1, y2));
            ToggleCorner(_corners, (x2, y1));
            ToggleCorner(_corners, (x2, y2));
        }

        public bool IsPerfectCover()
        {
            if (_totalArea != (long)(_maxX - _minX) * (_maxY - _minY) || _corners.Count != 4)
            {
                return false;
            }

            return _corners.Has((_minX, _minY)) && _corners.Has((_minX, _maxY))
                && _corners.Has((_maxX, _minY)) && _corners.Has((_maxX, _maxY));
        }
    }

    private static void ToggleCorner(Set<(int X, int Y)> corners, (int X, int Y) point)
    {
        if (!corners.TryAdd(point))
        {
            corners.TryRemove(point);
        }
    }
}
