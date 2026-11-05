namespace DSAExperimentation.LeetCode.Solutions.ClosestSubsequenceSum;

// The reach of ClosestSubsequenceSumSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one
// of them finishes in the harness's time. MinAbsoluteDifferenceByMeetInTheMiddle takes 6 ms over the 31 of
// the catalog's 86 cases with a length of nums up to 15 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class ClosestSubsequenceSumReach
{
    private const int LargestNumsLength = 15;

    public static bool IsWithin(int[] nums, int goal) =>
        nums.Length <= LargestNumsLength;
}
