namespace DSAExperimentation.LeetCode.Solutions.CircleAndRectangleOverlapping;

// The reach of CircleAndRectangleOverlappingSolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. HasOverlapByLatticePointScan tests every lattice point of the
// rectangle, and takes 132.4 ms over the 111 of the catalog's 114 cases whose rectangle spans at most
// 36,000,000 unit cells (measured 2026-11-05, Release); a rectangle of 10,000 by 10,000 takes it 244 ms alone.
internal static class CircleAndRectangleOverlappingReach
{
    private const long LargestRectangleCellCount = 36_000_000;

    public static bool IsWithin(Circle circle, Rectangle rectangle)
    {
        var width = (long)rectangle.X2 - rectangle.X1;
        var height = (long)rectangle.Y2 - rectangle.Y1;

        return width * height <= LargestRectangleCellCount;
    }
}
