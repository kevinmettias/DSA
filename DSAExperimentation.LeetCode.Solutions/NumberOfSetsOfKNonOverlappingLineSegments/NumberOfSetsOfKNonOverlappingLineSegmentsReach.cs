namespace DSAExperimentation.LeetCode.Solutions.NumberOfSetsOfKNonOverlappingLineSegments;

// The reach of NumberOfSetsOfKNonOverlappingLineSegmentsSolution's strategies (ARCHITECTURE 17.13): the
// catalog cases every one of them finishes in the harness's time. NumberOfSetsByMemoizedPascal takes 187.7 ms
// over the 43 of the catalog's 58 cases with a value of pointCount up to 700 (measured 2026-11-05, Release);
// the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class NumberOfSetsOfKNonOverlappingLineSegmentsReach
{
    private const int LargestPointCount = 700;

    public static bool IsWithin(int pointCount, int segmentCount) =>
        pointCount <= LargestPointCount;
}
