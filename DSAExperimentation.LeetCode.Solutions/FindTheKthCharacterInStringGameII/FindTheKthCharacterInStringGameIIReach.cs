namespace DSAExperimentation.LeetCode.Solutions.FindTheKthCharacterInStringGameII;

// The reach of FindTheKthCharacterInStringGameIISolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. KthCharacterByBruteForceSimulation takes 18.2 ms over the
// 21 of the catalog's 45 cases with a length of operations up to 20 (measured 2026-11-05, Release); the
// larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class FindTheKthCharacterInStringGameIIReach
{
    private const int LargestOperationsLength = 20;

    public static bool IsWithin(long targetPosition, int[] operations) =>
        operations.Length <= LargestOperationsLength;
}
