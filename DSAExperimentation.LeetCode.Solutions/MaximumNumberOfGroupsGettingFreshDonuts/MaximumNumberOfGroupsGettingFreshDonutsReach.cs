namespace DSAExperimentation.LeetCode.Solutions.MaximumNumberOfGroupsGettingFreshDonuts;

// The reach of MaximumNumberOfGroupsGettingFreshDonutsSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. MaxHappyGroupsByAllPermutations takes 212.4 ms over
// the 28 of the catalog's 120 cases with a length of groups up to 9 (measured 2026-11-05, Release); the
// larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class MaximumNumberOfGroupsGettingFreshDonutsReach
{
    private const int LargestGroupsLength = 9;

    public static bool IsWithin(int batchSize, int[] groups) =>
        groups.Length <= LargestGroupsLength;
}
