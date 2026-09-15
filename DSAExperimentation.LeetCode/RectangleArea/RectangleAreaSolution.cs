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
    public static long TotalAreaByClosedFormOverlap(Rectangle a, Rectangle b)
    {
        var overlapWidth = Math.Max(0, Math.Min(a.X2, b.X2) - Math.Max(a.X1, b.X1));
        var overlapHeight = Math.Max(0, Math.Min(a.Y2, b.Y2) - Math.Max(a.Y1, b.Y1));

        return a.Area + b.Area - (long)overlapWidth * overlapHeight;
    }

    public static long TotalAreaByUnitGridCoverageCount(Rectangle a, Rectangle b)
    {
        var bounds = BoundingRectangle(a, b);
        var covered = new bool[bounds.Width * bounds.Height];

        MarkRectangle(covered, bounds, a);
        MarkRectangle(covered, bounds, b);

        return CountCovered(covered);
    }

    // The scratch grid is indexed from the corner of the region both rectangles fall
    // inside, so the cells have to be counted from that corner rather than from zero.
    private static Rectangle BoundingRectangle(Rectangle a, Rectangle b) =>
        new(Math.Min(a.X1, b.X1),
            Math.Min(a.Y1, b.Y1),
            Math.Max(a.X2, b.X2),
            Math.Max(a.Y2, b.Y2));

    private static void MarkRectangle(bool[] covered, Rectangle bounds, Rectangle rectangle)
    {
        for (var y = rectangle.Y1; y < rectangle.Y2; y++)
        {
            for (var x = rectangle.X1; x < rectangle.X2; x++)
            {
                covered[((y - bounds.Y1) * bounds.Width) + (x - bounds.X1)] = true;
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
