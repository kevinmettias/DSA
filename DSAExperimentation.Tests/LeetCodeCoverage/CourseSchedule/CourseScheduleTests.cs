using DSAExperimentation.LeetCode.CourseSchedule;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CourseSchedule;

// Harness only. The single strategy is CourseScheduleSolution's - this file
// checks LeetCode's published examples, stated in LeetCode's own
// (numCourses, prerequisites) input shape.
public sealed class CourseScheduleTests
{
    public static TheoryData<int, int[][], bool> Examples =>
        new()
        {
            { 2, [[1, 0]], true },
            { 2, [[1, 0], [0, 1]], false },
            { 1, [], true },
            { 4, [[1, 0], [2, 0], [3, 1], [3, 2]], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanFinishByTopologicalSort_LeetCodeExamples_ReturnsWhetherCompletionIsPossible(
        int numCourses, int[][] prerequisites, bool expected) =>
        Assert.Equal(expected, CourseScheduleSolution.CanFinishByTopologicalSort(numCourses, prerequisites));
}
