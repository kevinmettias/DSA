using DSAExperimentation.LeetCode.Solutions.ParallelCoursesIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ParallelCoursesIII;

// Harness only. Both strategies are ParallelCoursesIIISolution's - this file pins
// them, in LeetCode's own (n, relations, time) input shape, to cases the two arms
// have to agree on: a prerequisite-free set whose answer is just the longest single
// duration, and a linear chain whose answer is the whole summed critical path.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class ParallelCoursesIIISolutionTests
{
    public static TheoryData<int, int[][], int[], int> Cases =>
        new()
        {
            { 4, [], [1, 2, 3, 4], 4 },
            { 3, [[1, 2], [2, 3]], [3, 2, 5], 10 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumTimeByKahnsTopologicalSortDp_Cases_ReturnsCriticalPathLength(
        int courseCount, int[][] relations, int[] time, int expected)
    {
        var actual = ParallelCoursesIIISolution.MinimumTimeByKahnsTopologicalSortDp(courseCount, relations, time);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumTimeByRepeatedRelaxation_Cases_ReturnsCriticalPathLength(
        int courseCount, int[][] relations, int[] time, int expected)
    {
        var actual = ParallelCoursesIIISolution.MinimumTimeByRepeatedRelaxation(courseCount, relations, time);

        Assert.Equal(expected, actual);
    }
}
