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
    public static long TotalAreaByClosedFormOverlap(Rectangle firstRectangle, Rectangle secondRectangle)
    {
        var overlapWidth = Math.Max(
            0, Math.Min(firstRectangle.X2, secondRectangle.X2) - Math.Max(firstRectangle.X1, secondRectangle.X1));
        var overlapHeight = Math.Max(
            0, Math.Min(firstRectangle.Y2, secondRectangle.Y2) - Math.Max(firstRectangle.Y1, secondRectangle.Y1));

        return firstRectangle.Area + secondRectangle.Area - (long)overlapWidth * overlapHeight;
    }

    public static long TotalAreaByUnitGridCoverageCount(Rectangle firstRectangle, Rectangle secondRectangle)
    {
        var bounds = BoundingRectangle(firstRectangle, secondRectangle);
        var covered = new bool[bounds.Width * bounds.Height];

        MarkRectangle(covered, bounds, firstRectangle);
        MarkRectangle(covered, bounds, secondRectangle);

        return CountCovered(covered);
    }

    // The scratch grid is indexed from the corner of the region both rectangles fall
    // inside, so the cells have to be counted from that corner rather than from zero.
    private static Rectangle BoundingRectangle(Rectangle firstRectangle, Rectangle secondRectangle) =>
        new(Math.Min(firstRectangle.X1, secondRectangle.X1),
            Math.Min(firstRectangle.Y1, secondRectangle.Y1),
            Math.Max(firstRectangle.X2, secondRectangle.X2),
            Math.Max(firstRectangle.Y2, secondRectangle.Y2));

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
