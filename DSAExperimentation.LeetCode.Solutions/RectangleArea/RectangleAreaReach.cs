namespace DSAExperimentation.LeetCode.Solutions.RectangleArea;

// The reach of RectangleAreaSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. TotalAreaByUnitGridCoverageCount counts every unit cell of the box both
// rectangles fit in, and takes 12.9 ms over the 70 of the catalog's 76 cases whose box spans at most
// 4,000,000 cells (measured 2026-11-05, Release); the next three, boxes of 10,000 by 10,000, add 522 ms.
internal static class RectangleAreaReach
{
    private const long LargestBoxCellCount = 4_000_000;

    public static bool IsWithin(Rectangle firstRectangle, Rectangle secondRectangle)
    {
        var width = (long)Math.Max(firstRectangle.X2, secondRectangle.X2) - Math.Min(firstRectangle.X1, secondRectangle.X1);
        var height = (long)Math.Max(firstRectangle.Y2, secondRectangle.Y2) - Math.Min(firstRectangle.Y1, secondRectangle.Y1);

        return width * height <= LargestBoxCellCount;
    }
}
