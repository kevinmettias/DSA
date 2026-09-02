namespace DSAExperimentation.LeetCode.RectangleArea;

// LeetCode 223. Rectangle Area: total area covered by two axis-aligned rectangles
// is area1 + area2 - overlap, where overlap is the product of each axis's clamped
// intersection length.
//
// The closed-form strategy is O(1) scalar arithmetic and composes no repo
// primitive at all - the same "lighter repo-primitive fit" this repo already
// accepted for Pow(x, n) / Power of Two. The unit-grid strategy is the naive
// brute force it has to beat: materialize every covered unit cell on a scratch
// grid and count them. Its internals stay a plain bool[] rather than this repo's
// DynamicArray<bool> - a baseline represents "what you would write without this
// repo", and there is no input container here for a repo type to occupy.
internal static class RectangleAreaSolution
{
    public static long TotalAreaByClosedFormOverlap(
        int ax1, int ay1, int ax2, int ay2, int bx1, int by1, int bx2, int by2)
    {
        var area1 = (long)(ax2 - ax1) * (ay2 - ay1);
        var area2 = (long)(bx2 - bx1) * (by2 - by1);

        var overlapWidth = Math.Max(0, Math.Min(ax2, bx2) - Math.Max(ax1, bx1));
        var overlapHeight = Math.Max(0, Math.Min(ay2, by2) - Math.Max(ay1, by1));

        return area1 + area2 - (long)overlapWidth * overlapHeight;
    }

    public static long TotalAreaByUnitGridCoverageCount(
        int ax1, int ay1, int ax2, int ay2, int bx1, int by1, int bx2, int by2)
    {
        var minX = Math.Min(ax1, bx1);
        var minY = Math.Min(ay1, by1);
        var maxX = Math.Max(ax2, bx2);
        var maxY = Math.Max(ay2, by2);

        var width = maxX - minX;
        var height = maxY - minY;
        var covered = new bool[width * height];

        MarkRectangle(covered, width, minX, minY, ax1, ay1, ax2, ay2);
        MarkRectangle(covered, width, minX, minY, bx1, by1, bx2, by2);

        return CountCovered(covered);
    }

    private static void MarkRectangle(
        bool[] covered, int width, int minX, int minY, int x1, int y1, int x2, int y2)
    {
        for (var y = y1; y < y2; y++)
        {
            for (var x = x1; x < x2; x++)
            {
                covered[((y - minY) * width) + (x - minX)] = true;
            }
        }
    }

    private static long CountCovered(bool[] covered)
    {
        long count = 0;

        foreach (var cell in covered)
        {
            if (cell)
            {
                count++;
            }
        }

        return count;
    }
}
