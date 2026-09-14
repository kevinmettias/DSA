using DSAExperimentation.LeetCode.ParallelCoursesIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ParallelCoursesIII;

// Harness only. Both strategies are ParallelCoursesIIISolution's - this file pins
// them to LeetCode's published examples in LeetCode's own (n, relations, time)
// input shape, plus the cases the two arms have to agree on: a single course, a
// prerequisite-free set whose answer is just the longest single duration, and a
// linear chain whose answer is the whole summed critical path.
public sealed class ParallelCoursesIIITests
{
    public static TheoryData<int, int[][], int[], int> Examples =>
        new()
        {
            { 3, [[1, 3], [2, 3]], [3, 2, 5], 8 },
            { 5, [[1, 5], [2, 5], [3, 5], [3, 4], [4, 5]], [1, 2, 3, 4, 5], 12 },
            { 1, [], [5], 5 },
            { 4, [], [1, 2, 3, 4], 4 },
            { 3, [[1, 2], [2, 3]], [3, 2, 5], 10 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumTimeByKahnsTopologicalSortDp_LeetCodeExamples_ReturnsCriticalPathLength(
        int n, int[][] relations, int[] time, int expected) =>
        Assert.Equal(expected, ParallelCoursesIIISolution.MinimumTimeByKahnsTopologicalSortDp(n, relations, time));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumTimeByRepeatedRelaxation_LeetCodeExamples_ReturnsCriticalPathLength(
        int n, int[][] relations, int[] time, int expected) =>
        Assert.Equal(expected, ParallelCoursesIIISolution.MinimumTimeByRepeatedRelaxation(n, relations, time));
}
