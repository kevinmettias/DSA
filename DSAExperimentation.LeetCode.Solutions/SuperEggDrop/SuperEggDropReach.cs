namespace DSAExperimentation.LeetCode.Solutions.SuperEggDrop;

// The reach of SuperEggDropSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. MinMovesByLinearScan takes 20.2 ms over the 23 of the catalog's 92 cases
// with a value of floors up to 150 (measured 2026-11-05, Release); the larger cases run past the 250 ms a
// strategy's catalog cases are allowed.
internal static class SuperEggDropReach
{
    private const int LargestFloors = 150;

    public static bool IsWithin(int eggs, int floors) =>
        floors <= LargestFloors;
}
