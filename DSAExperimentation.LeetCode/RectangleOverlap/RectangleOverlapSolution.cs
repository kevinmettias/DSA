namespace DSAExperimentation.LeetCode.RectangleOverlap;

// LeetCode 836. Rectangle Overlap: two axis-aligned rectangles overlap with
// positive area exactly when both axes' intervals overlap strictly.
//
// The closed-form strategy is O(1) scalar arithmetic and composes no repo
// primitive at all - the same "lighter repo-primitive fit" RectangleAreaSolution
// already documents for LC 223, whose closed-form overlap arithmetic this is the
// boolean half of. The unit-grid strategy is the naive brute force it has to
// beat: materialize rectangle 1's covered unit cells on a scratch grid, then scan
// rectangle 2's cells for a marked one. Its internals stay a plain bool[] rather
// than this repo's DynamicArray<bool>, for the reason RectangleAreaSolution gives
// - a baseline represents "what you would write without this repo", and the only
// input container here is LeetCode's own int[] pair.
internal static class RectangleOverlapSolution
{
    private const int X1 = 0;
    private const int Y1 = 1;
    private const int X2 = 2;
    private const int Y2 = 3;

    // Positive-area overlap on each axis independently: the intervals [x1, x2) and
    // [y1, y2) must each overlap strictly, so touching edges report false.
    public static bool OverlapsByClosedFormAxisIntervals(int[] rec1, int[] rec2) =>
        rec1[X1] < rec2[X2] && rec2[X1] < rec1[X2] &&
        rec1[Y1] < rec2[Y2] && rec2[Y1] < rec1[Y2];

    // The textbook brute force: paint every unit cell rectangle 1 covers onto a
    // scratch grid spanning both rectangles' bounding box, then walk rectangle 2's
    // cells looking for one already painted. O(width * height) in the bounding box
    // rather than O(1), and it answers the same question - a shared cell exists
    // exactly when the two rectangles share positive area.
    public static bool OverlapsByUnitGridIntersectionScan(int[] rec1, int[] rec2)
    {
        var minX = Math.Min(rec1[X1], rec2[X1]);
        var minY = Math.Min(rec1[Y1], rec2[Y1]);
        var maxX = Math.Max(rec1[X2], rec2[X2]);
        var maxY = Math.Max(rec1[Y2], rec2[Y2]);

        var width = maxX - minX;
        var height = maxY - minY;
        var covered = new bool[width * height];

        MarkRectangle(covered, width, (minX, minY), rec1);

        return IntersectsMarkedGrid(covered, width, (minX, minY), rec2);
    }

    private static void MarkRectangle(bool[] covered, int width, (int X, int Y) origin, int[] rect)
    {
        for (var y = rect[Y1]; y < rect[Y2]; y++)
        {
            for (var x = rect[X1]; x < rect[X2]; x++)
            {
                covered[((y - origin.Y) * width) + (x - origin.X)] = true;
            }
        }
    }

    private static bool IntersectsMarkedGrid(bool[] covered, int width, (int X, int Y) origin, int[] rect)
    {
        for (var y = rect[Y1]; y < rect[Y2]; y++)
        {
            for (var x = rect[X1]; x < rect[X2]; x++)
            {
                if (covered[((y - origin.Y) * width) + (x - origin.X)])
                {
                    return true;
                }
            }
        }

        return false;
    }
}
