namespace DSAExperimentation.LeetCode.Solutions.CountIncreasingQuadruplets;

// The reach of CountIncreasingQuadrupletsSolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. CountQuadrupletsByBruteForce takes 161.4 ms over the 83 of the
// catalog's 85 cases with a length of nums up to 251 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class CountIncreasingQuadrupletsReach
{
    private const int LargestNumsLength = 251;

    public static bool IsWithin(int[] nums) =>
        nums.Length <= LargestNumsLength;
}
