namespace DSAExperimentation.LeetCode.Solutions.RaceCar;

// The reach of RaceCarSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. MinCommandsByReduceGraph takes 159.5 ms over the 24 of the catalog's 44
// cases with a value of target up to 250 (measured 2026-11-05, Release); the larger cases run past the 250 ms
// a strategy's catalog cases are allowed.
internal static class RaceCarReach
{
    private const int LargestTarget = 250;

    public static bool IsWithin(int target) =>
        target <= LargestTarget;
}
