namespace DSAExperimentation.LeetCode.Solutions.AirplaneSeatAssignmentProbability;

// The reach of AirplaneSeatAssignmentProbabilitySolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. NthPersonGetsNthSeatByMemoizedRecursion takes 207 ms over
// the 16 of the catalog's 26 cases with a value of planeSize up to 2,500 (measured 2026-11-05, Release); the
// larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class AirplaneSeatAssignmentProbabilityReach
{
    private const int LargestPlaneSize = 2500;

    public static bool IsWithin(int planeSize) =>
        planeSize <= LargestPlaneSize;
}
