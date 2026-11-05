namespace DSAExperimentation.LeetCode.Solutions.FindMinimumTimeToFinishAllJobs;

// The reach of FindMinimumTimeToFinishAllJobsSolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. MinimumTimeByExhaustiveAssignment takes 8.4 ms over the 5
// of the catalog's 100 cases with a length of jobs up to 6 (measured 2026-11-05, Release); the larger cases
// run past the 250 ms a strategy's catalog cases are allowed.
internal static class FindMinimumTimeToFinishAllJobsReach
{
    private const int LargestJobsLength = 6;

    public static bool IsWithin(int[] jobs, int workerCount) =>
        jobs.Length <= LargestJobsLength;
}
