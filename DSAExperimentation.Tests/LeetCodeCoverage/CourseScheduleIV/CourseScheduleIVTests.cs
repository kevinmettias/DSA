using DSAExperimentation.LeetCode.CourseScheduleIV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CourseScheduleIV;

// Harness only. The prerequisite network is CourseGraph and both reachability
// strategies are CourseScheduleIVSolution's - this file just pins them to
// LeetCode's published examples plus the longer-chain and branching cases that
// separate a direct prerequisite from a transitive one.
public sealed partial class CourseScheduleIVTests
{
    public static TheoryData<int, int[][], int[][], bool[]> Examples =>
        new()
        {
            { 2, [[1, 0]], [[0, 1], [1, 0]], [false, true] },
            { 2, [], [[1, 0], [0, 1]], [false, false] },
            { 3, [[1, 2], [1, 0], [2, 0]], [[1, 0], [1, 2]], [true, true] },
            { 5, [[0, 1], [1, 2], [2, 3], [3, 4]], [[0, 4], [4, 0], [1, 3]], [true, false, true] },
            { 4, [[0, 1], [0, 2], [1, 3]], [[0, 3], [2, 3], [3, 0]], [true, false, false] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckIfPrerequisiteByBreadthFirstSearchPerQuery_LeetCodeExamples_AnswersEveryQuery(
        int numCourses, int[][] prerequisites, int[][] queries, bool[] expected)
    {
        var actual = CourseScheduleIVSolution.CheckIfPrerequisiteByBreadthFirstSearchPerQuery(
            numCourses, prerequisites, queries);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckIfPrerequisiteByFloydWarshall_LeetCodeExamples_AnswersEveryQuery(
        int numCourses, int[][] prerequisites, int[][] queries, bool[] expected)
    {
        var actual = CourseScheduleIVSolution.CheckIfPrerequisiteByFloydWarshall(
            numCourses, prerequisites, queries);
        Assert.Equal(expected, actual);
    }
}
