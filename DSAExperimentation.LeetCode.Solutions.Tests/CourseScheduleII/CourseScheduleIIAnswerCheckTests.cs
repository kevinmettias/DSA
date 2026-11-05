using DSAExperimentation.LeetCode.Solutions.CourseScheduleII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CourseScheduleII;

public sealed partial class CourseScheduleIIAnswerCheckTests
{
    public static TheoryData<int, int[][], int[], int[], bool> Answers =>
        new()
        {
            { 4, [[1, 0], [2, 0], [3, 1], [3, 2]], [0, 2, 1, 3], [0, 2, 1, 3], true },
            { 4, [[1, 0], [2, 0], [3, 1], [3, 2]], [0, 1, 2, 3], [0, 2, 1, 3], true },
            { 2, [[1, 0], [0, 1]], [], [], true },
            { 4, [[1, 0], [2, 0], [3, 1], [3, 2]], [0, 1, 3, 2], [0, 2, 1, 3], false },
            { 4, [[1, 0], [2, 0], [3, 1], [3, 2]], [0, 1, 2], [0, 2, 1, 3], false },
            { 4, [[1, 0], [2, 0], [3, 1], [3, 2]], [0, 1, 1, 3], [0, 2, 1, 3], false },
            { 4, [[1, 0], [2, 0], [3, 1], [3, 2]], [], [0, 2, 1, 3], false },
            { 2, [[1, 0], [0, 1]], [0, 1], [], false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForAnOrderTakingEachCourseAfterItsPrerequisites(int numCourses, int[][] prerequisites, int[] answer, int[] recorded, bool expected)
        => Assert.Equal(expected, CourseScheduleIIAnswerCheck.IsAccepted(numCourses, prerequisites, answer, recorded));
}
