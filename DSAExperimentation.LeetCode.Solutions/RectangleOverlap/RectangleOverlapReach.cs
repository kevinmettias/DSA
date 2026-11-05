namespace DSAExperimentation.LeetCode.Solutions.RectangleOverlap;

// The reach of RectangleOverlapSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. IsOverlappingByUnitGridIntersectionScan lays a grid over the box both
// rectangles fit in, and takes 11.5 ms over the 93 of the catalog's 99 cases whose box spans at most
// 9,000,000 unit cells (measured 2026-11-05, Release); past them, boxes of up to 2 * 10^9 on a side, the grid
// is too large to allocate or to index with an int.
internal static class RectangleOverlapReach
{
    private const long LargestBoxCellCount = 9_000_000;

    public static bool IsWithin(int[] rec1, int[] rec2)
    {
        var width = (long)Math.Max(rec1[2], rec2[2]) - Math.Min(rec1[0], rec2[0]);
        var height = (long)Math.Max(rec1[3], rec2[3]) - Math.Min(rec1[1], rec2[1]);

        return width * height <= LargestBoxCellCount;
    }
}
