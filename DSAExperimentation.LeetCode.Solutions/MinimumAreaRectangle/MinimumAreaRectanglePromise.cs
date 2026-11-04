namespace DSAExperimentation.LeetCode.Solutions.MinimumAreaRectangle;

// LC 939's promise about its input: all the points are unique. LeetCode's judge never poses an
// input that breaks it. A repeated point makes a rectangle of no area, which the strategies each
// read their own way. Generated cases do not always keep the promise, so a harness that runs them
// asks this first.
internal static class MinimumAreaRectanglePromise
{
    public static bool IsKeptBy(int[][] points) => points.Select(point => (point[0], point[1])).Distinct().Count() == points.Length;
}
