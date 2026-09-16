using DSAExperimentation.LeetCode.ParallelCoursesII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ParallelCoursesII;

// Harness only. Both strategies are ParallelCoursesIISolution's - LeetCode's
// published examples are stated once and replayed against each, so a failure names
// the strategy that broke rather than reporting a disagreement between an anonymous
// test helper and an anonymous benchmark arm. The no-prerequisites cases are the
// shape the benchmark measures, so they are asserted here too.
public sealed class ParallelCoursesIITests
{
    // Cases both arms are checked at. The unmemoized arm re-explores a
    // completed-course mask once per semester order that reaches it (courseCount = 11
    // with no prerequisites already costs ~9.7e8 calls), so the shared set stops at
    // courseCount = 8 and the memoized arm gets the larger case below.
    public static TheoryData<int, int[][], int, int> Examples =>
        new()
        {
            { 4, [[2, 1], [3, 1], [1, 4]], 2, 3 },
            { 5, [[2, 1], [3, 1], [4, 1], [1, 5]], 2, 4 },
            { 1, [], 1, 1 },
            { 3, [[1, 2], [2, 3]], 3, 3 },
            { 6, [], 3, 2 },
            { 6, [[1, 2], [1, 3], [1, 4], [1, 5], [1, 6]], 2, 4 },
            { 8, [], 2, 4 },
        };

    // The pre-migration test's own packing case, kept at the size it was written
    // at - only the memoized arm reaches it in test time.
    public static TheoryData<int, int[][], int, int> LargeExamples =>
        new()
        {
            { 11, [], 2, 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinNumberOfSemestersByBruteForceRecursion_LeetCodeExamples_ReturnsFewestSemesters(
        int courseCount, int[][] relations, int maxPerSemester, int expected)
    {
        var semesters = ParallelCoursesIISolution.MinNumberOfSemestersByBruteForceRecursion(
            courseCount, relations, maxPerSemester);

        Assert.Equal(expected, semesters);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinNumberOfSemestersByMemoizedRecursion_LeetCodeExamples_ReturnsFewestSemesters(
        int courseCount, int[][] relations, int maxPerSemester, int expected)
    {
        var semesters = ParallelCoursesIISolution.MinNumberOfSemestersByMemoizedRecursion(
            courseCount, relations, maxPerSemester);

        Assert.Equal(expected, semesters);
    }

    [Theory]
    [MemberData(nameof(LargeExamples))]
    public void MinNumberOfSemestersByMemoizedRecursion_LargeCourseCounts_ReturnsFewestSemesters(
        int courseCount, int[][] relations, int maxPerSemester, int expected)
    {
        var semesters = ParallelCoursesIISolution.MinNumberOfSemestersByMemoizedRecursion(
            courseCount, relations, maxPerSemester);

        Assert.Equal(expected, semesters);
    }
}
