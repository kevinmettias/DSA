namespace DSAExperimentation.LeetCode.Solutions.FindTheSumOfSubsequencePowers;

// The reach of FindTheSumOfSubsequencePowersSolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. SumOfPowersByThresholdCounting takes 7.3 ms over the 31
// of the catalog's 114 cases with a length of nums up to 7 (measured 2026-11-05, Release); the larger cases
// run past the 250 ms a strategy's catalog cases are allowed.
internal static class FindTheSumOfSubsequencePowersReach
{
    private const int LargestNumsLength = 7;

    public static bool IsWithin(int[] nums, int subsequenceLength) =>
        nums.Length <= LargestNumsLength;
}
