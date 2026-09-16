using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.PerfectRectangle;

// LeetCode 391. Perfect Rectangle: does a set of axis-aligned rectangles exactly
// tile their combined bounding box, with no gaps and no overlaps?
//
// Both strategies share the same O(n) area-vs-bounding-box check, which alone
// catches every gap: total area falls short of the bounding box's area whenever
// anything is left uncovered. Overlaps need a second signal, since an overlap's
// excess area can be masked by an equal-sized gap elsewhere. The brute-force
// strategy checks every rectangle pair directly for that second signal - the
// textbook O(n^2) answer this repo's own corner-toggle strategy has to beat. The
// corner-toggle strategy instead toggles each rectangle's four corners in a
// Set<(int,int)> (this repo's own HashMap<T,bool> composition): every interior
// corner of a genuine tiling is shared by an even number of rectangles and so
// toggles back out, leaving only the bounding box's own four corners standing - an
// O(n) check that catches overlaps the area check alone would miss.
internal static class PerfectRectangleSolution
{
    private const int X2Index = 2;
    private const int Y2Index = 3;

    public static bool IsRectangleCoverByPairwiseOverlap(int[][] rectangles)
    {
        for (var i = 0; i < rectangles.Length; i++)
        {
            for (var j = i + 1; j < rectangles.Length; j++)
            {
                if (IsOverlapping(rectangles[i], rectangles[j]))
                {
                    return false;
                }
            }
        }

        return HasMatchingBoundingBoxArea(rectangles);
    }

    private static bool IsOverlapping(int[] firstRectangle, int[] secondRectangle)
        => firstRectangle[0] < secondRectangle[X2Index] && secondRectangle[0] < firstRectangle[X2Index]
            && firstRectangle[1] < secondRectangle[Y2Index] && secondRectangle[1] < firstRectangle[Y2Index];

    private static bool HasMatchingBoundingBoxArea(int[][] rectangles)
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;
        long totalArea = 0;

        foreach (var rect in rectangles)
        {
            minX = Math.Min(minX, rect[0]);
            minY = Math.Min(minY, rect[1]);
            maxX = Math.Max(maxX, rect[X2Index]);
            maxY = Math.Max(maxY, rect[Y2Index]);
            totalArea += (long)(rect[X2Index] - rect[0]) * (rect[Y2Index] - rect[1]);
        }

        return totalArea == (long)(maxX - minX) * (maxY - minY);
    }

    public static bool IsRectangleCoverByCornerToggle(int[][] rectangles)
    {
        var accumulator = new RectangleAccumulator();

        foreach (var rect in rectangles)
        {
            accumulator.Add(rect);
        }

        return accumulator.IsPerfectCover();
    }

    // The brute-force strategy's pairing-check counterpart: toggling four corners
    // per rectangle and re-deriving the bounding box from the same running min/max
    // this repo's other strategy computes with a foreach loop instead of a class.
    private sealed class RectangleAccumulator
    {
        private const int PerfectRectangleCornerCount = 4;

        private readonly Set<(int X, int Y)> _corners = new();
        private int _minX = int.MaxValue;
        private int _minY = int.MaxValue;
        private int _maxX = int.MinValue;
        private int _maxY = int.MinValue;
        private long _totalArea;

        public void Add(int[] rect)
        {
            var (x1, y1, x2, y2) = (rect[0], rect[1], rect[X2Index], rect[Y2Index]);

            AccumulateExtentsAndArea(x1, y1, x2, y2);
            ToggleCorners(x1, y1, x2, y2);
        }

        // Grow the running bounding box to take in this rectangle, and charge its own
        // area to the running total.
        private void AccumulateExtentsAndArea(int x1, int y1, int x2, int y2)
        {
            _minX = Math.Min(_minX, x1);
            _minY = Math.Min(_minY, y1);
            _maxX = Math.Max(_maxX, x2);
            _maxY = Math.Max(_maxY, y2);
            _totalArea += (long)(x2 - x1) * (y2 - y1);
        }

        // Every corner is toggled in or out of the set, so a corner shared by an even
        // number of rectangles cancels back out.
        private void ToggleCorners(int x1, int y1, int x2, int y2)
        {
            ToggleCorner(_corners, (x1, y1));
            ToggleCorner(_corners, (x1, y2));
            ToggleCorner(_corners, (x2, y1));
            ToggleCorner(_corners, (x2, y2));
        }

        private static void ToggleCorner(Set<(int X, int Y)> corners, (int X, int Y) point)
        {
            if (!corners.TryAdd(point))
            {
                corners.TryRemove(point);
            }
        }

        public bool IsPerfectCover()
        {
            if (_totalArea != (long)(_maxX - _minX) * (_maxY - _minY) ||
                _corners.Count != PerfectRectangleCornerCount)
            {
                return false;
            }

            return _corners.Has((_minX, _minY)) && _corners.Has((_minX, _maxY))
                && _corners.Has((_maxX, _minY)) && _corners.Has((_maxX, _maxY));
        }
    }
}
