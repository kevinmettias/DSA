namespace DSAExperimentation.LeetCode.Solutions.CountLatticePointsInsideACircle;

// LC 2249's promise about its input: every circle lies in the first quadrant: its radius is at most
// its center's smaller coordinate. LeetCode's judge never poses an input that breaks it. The
// strategies scan only the first quadrant, so a circle reaching past an axis loses the points
// beyond it. Generated cases do not always keep the promise, so a harness that runs them asks this
// first.
internal static class CountLatticePointsInsideACirclePromise
{
    private const int CenterX = 0;
    private const int CenterY = 1;
    private const int Radius = 2;

    public static bool IsKeptBy(int[][] circles)
        => circles.All(circle => circle[Radius] <= Math.Min(circle[CenterX], circle[CenterY]));
}
