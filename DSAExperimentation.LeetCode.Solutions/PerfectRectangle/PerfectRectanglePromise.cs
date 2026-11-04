namespace DSAExperimentation.LeetCode.Solutions.PerfectRectangle;

// LC 391's promise about its input: every rectangle has a positive width and height. LeetCode's
// judge never poses an input that breaks it. A rectangle of no width has corners the corner toggle
// counts and no area the overlap sees. Generated cases do not always keep the promise, so a harness
// that runs them asks this first.
internal static class PerfectRectanglePromise
{
    public static bool IsKeptBy(int[][] rectangles)
        => rectangles.All(rectangle => rectangle[0] < rectangle[PerfectRectangleSolution.X2Index] && rectangle[1] < rectangle[PerfectRectangleSolution.Y2Index]);
}
