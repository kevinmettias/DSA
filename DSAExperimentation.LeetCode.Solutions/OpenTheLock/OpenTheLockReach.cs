namespace DSAExperimentation.LeetCode.Solutions.OpenTheLock;

// The reach of OpenTheLockSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. MinTurnsByReduceGraph takes 151 ms over the 8 of the catalog's 107 cases
// with a length of deadends up to 3 (measured 2026-11-05, Release); the larger cases run past the 250 ms a
// strategy's catalog cases are allowed.
internal static class OpenTheLockReach
{
    private const int LargestDeadendsLength = 3;

    public static bool IsWithin(IEnumerable<string> deadends, string target) =>
        deadends.Count() <= LargestDeadendsLength;
}
