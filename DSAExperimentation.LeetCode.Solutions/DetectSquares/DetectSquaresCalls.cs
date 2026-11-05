namespace DSAExperimentation.LeetCode.Solutions.DetectSquares;

// LeetCode's two calls, which pass a point as one [x, y] array where IDetectSquares takes its two coordinates
// (ARCHITECTURE 17.13).
internal static class DetectSquaresCalls
{
    private const int X = 0;
    private const int Y = 1;

    public static void Add(DetectSquaresSolution.IDetectSquares squares, int[] point) => squares.Add(point[X], point[Y]);

    public static int Count(DetectSquaresSolution.IDetectSquares squares, int[] point) => squares.Count(point[X], point[Y]);
}
