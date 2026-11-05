namespace DSAExperimentation.LeetCode.Solutions.MinimumNumberOfArrowsToBurstBalloons;

// The reach of MinimumNumberOfArrowsToBurstBalloonsSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. FindMinArrowShotsBySortEndsThenGreedyScan takes 3.7
// ms over the 61 of the catalog's 106 cases with a length of points up to 9 (measured 2026-11-05, Release);
// the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class MinimumNumberOfArrowsToBurstBalloonsReach
{
    private const int LargestPointsLength = 9;

    public static bool IsWithin(int[][] points) =>
        points.Length <= LargestPointsLength;
}
