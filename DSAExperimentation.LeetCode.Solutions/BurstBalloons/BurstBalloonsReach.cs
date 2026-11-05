namespace DSAExperimentation.LeetCode.Solutions.BurstBalloons;

// The reach of BurstBalloonsSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. MaxCoinsByUnmemoizedRecursion takes 15.3 ms over the 44 of the catalog's 87
// cases with a length of nums up to 12 (measured 2026-11-05, Release); the larger cases run past the 250 ms a
// strategy's catalog cases are allowed.
internal static class BurstBalloonsReach
{
    private const int LargestNumsLength = 12;

    public static bool IsWithin(int[] nums) =>
        nums.Length <= LargestNumsLength;
}
