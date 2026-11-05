namespace DSAExperimentation.LeetCode.Solutions.MinimumNumberOfWorkSessionsToFinishTheTasks;

// The reach of MinimumNumberOfWorkSessionsToFinishTheTasksSolution's strategies (ARCHITECTURE 17.13): the
// catalog cases every one of them finishes in the harness's time. MinSessionsByUnmemoizedRecursion takes 50
// ms over the 20 of the catalog's 95 cases with a length of tasks up to 8 (measured 2026-11-05, Release); the
// larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class MinimumNumberOfWorkSessionsToFinishTheTasksReach
{
    private const int LargestTasksLength = 8;

    public static bool IsWithin(int[] tasks, int sessionTime) =>
        tasks.Length <= LargestTasksLength;
}
