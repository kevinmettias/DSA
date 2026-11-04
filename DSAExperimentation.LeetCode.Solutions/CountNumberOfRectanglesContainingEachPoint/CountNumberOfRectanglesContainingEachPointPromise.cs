namespace DSAExperimentation.LeetCode.Solutions.CountNumberOfRectanglesContainingEachPoint;

// LC 2250's promise about its input: every rectangle's height and every point's height is at most
// 100. LeetCode's judge never poses an input that breaks it. The grouped search files rectangles by
// height and asks only the heights up to 100. Generated cases do not always keep the promise, so a
// harness that runs them asks this first.
internal static class CountNumberOfRectanglesContainingEachPointPromise
{
    private const int MostHeight = 100;
    private const int Height = 1;

    public static bool IsKeptBy(int[][] rectangles, int[][] points)
        => rectangles.Concat(points).All(shape => shape[Height] <= MostHeight);
}
