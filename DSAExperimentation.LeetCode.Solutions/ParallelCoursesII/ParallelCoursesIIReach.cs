namespace DSAExperimentation.LeetCode.Solutions.ParallelCoursesII;

// The reach of ParallelCoursesIISolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. MinNumberOfSemestersByBruteForceRecursion takes 28.5 ms over the 43 of
// the catalog's 92 cases with a value of courseCount up to 10 and a length of relations up to 13 (measured
// 2026-11-05, Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class ParallelCoursesIIReach
{
    private const int LargestCourseCount = 10;
    private const int LargestRelationsLength = 13;

    public static bool IsWithin(int courseCount, int[][] relations, int maxPerSemester) =>
        courseCount <= LargestCourseCount && relations.Length <= LargestRelationsLength;
}
