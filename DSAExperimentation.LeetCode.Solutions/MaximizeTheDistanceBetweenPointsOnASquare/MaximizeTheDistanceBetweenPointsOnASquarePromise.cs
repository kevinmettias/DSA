namespace DSAExperimentation.LeetCode.Solutions.MaximizeTheDistanceBetweenPointsOnASquare;

// LC 3464's promise about its input: every point lies on the boundary of the square, and no point
// repeats. LeetCode's judge never poses an input that breaks it. The strategies measure along the
// boundary, which a point inside the square is not on. Generated cases do not always keep the
// promise, so a harness that runs them asks this first.
internal static class MaximizeTheDistanceBetweenPointsOnASquarePromise
{
    public static bool IsKeptBy(int side, int[][] points, int selectionCount)
        => points.All(point => IsOnBoundary(side, point[0], point[1])) && points.Select(point => (point[0], point[1])).Distinct().Count() == points.Length;

    private static bool IsOnBoundary(int side, int across, int up) => IsInside(side, across, up) && IsOnASide(side, across, up);

    private static bool IsInside(int side, int across, int up) => IsWithin(side, across) && IsWithin(side, up);

    private static bool IsWithin(int side, int coordinate) => coordinate >= 0 && coordinate <= side;

    private static bool IsOnASide(int side, int across, int up) => IsOnAnEdge(side, across) || IsOnAnEdge(side, up);

    private static bool IsOnAnEdge(int side, int coordinate) => coordinate == 0 || coordinate == side;
}
