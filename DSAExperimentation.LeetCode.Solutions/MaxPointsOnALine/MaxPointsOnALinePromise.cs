namespace DSAExperimentation.LeetCode.Solutions.MaxPointsOnALine;

// LC 149's promise about its input: all the points are unique. LeetCode's judge never poses an
// input that breaks it. A repeated point has no slope to its copy, which the slope grouping and the
// cross product count differently. Generated cases do not always keep the promise, so a harness
// that runs them asks this first.
internal static class MaxPointsOnALinePromise
{
    public static bool IsKeptBy(int[][] points) => points.Select(point => (point[0], point[1])).Distinct().Count() == points.Length;
}
