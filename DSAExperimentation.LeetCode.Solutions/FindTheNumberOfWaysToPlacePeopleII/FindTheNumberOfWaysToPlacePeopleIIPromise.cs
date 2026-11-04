namespace DSAExperimentation.LeetCode.Solutions.FindTheNumberOfWaysToPlacePeopleII;

// LC 3027's promise about its input: all the points are distinct. LeetCode's judge never poses an
// input that breaks it. Two people on one point fence each other in a way the brute force and the
// sweep count differently. Generated cases do not always keep the promise, so a harness that runs
// them asks this first.
internal static class FindTheNumberOfWaysToPlacePeopleIIPromise
{
    public static bool IsKeptBy(int[][] points) => points.Select(point => (point[0], point[1])).Distinct().Count() == points.Length;
}
