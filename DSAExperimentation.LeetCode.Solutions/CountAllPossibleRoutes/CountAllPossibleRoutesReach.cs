namespace DSAExperimentation.LeetCode.Solutions.CountAllPossibleRoutes;

// The reach of CountAllPossibleRoutesSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one
// of them finishes in the harness's time. CountRoutesByNaiveRecursion takes 151.8 ms over the 21 of the
// catalog's 60 cases with a length of locations up to 8 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class CountAllPossibleRoutesReach
{
    private const int LargestLocationsLength = 8;

    public static bool IsWithin(int[] locations, int start, int finish, int fuel) =>
        locations.Length <= LargestLocationsLength;
}
