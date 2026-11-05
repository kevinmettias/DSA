using DSAExperimentation.LeetCode.Solutions.CourseScheduleIV;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CourseScheduleIV;

// Harness only. The prerequisite network is CourseGraph and both reachability
// strategies are CourseScheduleIVSolution's - this file just pins them to
// the longer-chain and branching cases that
// separate a direct prerequisite from a transitive one.
public sealed partial class CourseScheduleIVSolutionTests
{
    public static TheoryData<int, int[][], int[][], bool[]> Cases =>
        new()
        {
            { 4, [[0, 1], [0, 2], [1, 3]], [[0, 3], [2, 3], [3, 0]], [true, false, false] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CheckIfPrerequisiteByBreadthFirstSearchPerQuery_Cases_AnswersEveryQuery(
        int numCourses, int[][] prerequisites, int[][] queries, bool[] expected)
    {
        var actual = CourseScheduleIVSolution.CheckIfPrerequisiteByBreadthFirstSearchPerQuery(
            numCourses, prerequisites, queries);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CheckIfPrerequisiteByFloydWarshall_Cases_AnswersEveryQuery(
        int numCourses, int[][] prerequisites, int[][] queries, bool[] expected)
    {
        var actual = CourseScheduleIVSolution.CheckIfPrerequisiteByFloydWarshall(
            numCourses, prerequisites, queries);
        Assert.Equal(expected, actual);
    }
}
