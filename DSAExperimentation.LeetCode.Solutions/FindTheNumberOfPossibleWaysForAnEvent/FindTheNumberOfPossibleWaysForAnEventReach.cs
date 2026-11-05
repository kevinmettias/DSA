namespace DSAExperimentation.LeetCode.Solutions.FindTheNumberOfPossibleWaysForAnEvent;

// The reach of FindTheNumberOfPossibleWaysForAnEventSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. NumberOfWaysByBruteForceEnumeration walks every one
// of the stageCount^n stage assignments, and takes 90.6 ms over the 20 of the catalog's 91 cases with at most
// 1,000,000 of them (measured 2026-11-05, Release); the next, 5^10 assignments, takes it 546 ms alone.
internal static class FindTheNumberOfPossibleWaysForAnEventReach
{
    private const long LargestAssignmentCount = 1_000_000;

    public static bool IsWithin(int performerCount, int stageCount, int maxScore)
    {
        var assignments = 1L;

        for (var performer = 0; performer < performerCount && assignments <= LargestAssignmentCount; performer++)
        {
            assignments *= stageCount;
        }

        return assignments <= LargestAssignmentCount;
    }
}
