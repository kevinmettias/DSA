namespace DSAExperimentation.LeetCode.Solutions.CountSubtreesWithMaxDistanceBetweenCities;

// The reach of CountSubtreesWithMaxDistanceBetweenCitiesSolution's strategies (ARCHITECTURE 17.13): the
// catalog cases every one of them finishes in the harness's time. CountSubtreesByAllPairsBfs takes 195.4 ms
// over the 41 of the catalog's 45 cases with a value of cityCount up to 14 (measured 2026-11-05, Release);
// the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class CountSubtreesWithMaxDistanceBetweenCitiesReach
{
    private const int LargestCityCount = 14;

    public static bool IsWithin(int cityCount, int[][] edges) =>
        cityCount <= LargestCityCount;
}
