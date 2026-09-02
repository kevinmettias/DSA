using DSAExperimentation.LeetCode.CourseScheduleII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CourseScheduleII;

// Harness only. Both strategies are CourseScheduleIISolution's - this file pins
// them to LeetCode's published examples, stated in LeetCode's own (numCourses,
// prerequisites) input shape. Multiple orderings can satisfy the same
// prerequisites (the diamond-dependency example accepts both [0,1,2,3] and
// [0,2,1,3]), so a returned order is checked against the prerequisite
// constraints themselves rather than against one fixed expected array.
public sealed class CourseScheduleIITests
{
    public static TheoryData<int, int[][], bool> Examples =>
        new()
        {
            { 2, [[1, 0]], true },
            { 2, [[1, 0], [0, 1]], false },
            { 4, [[1, 0], [2, 0], [3, 1], [3, 2]], true },
            { 1, [], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindOrderByKahnsTopologicalSort_LeetCodeExamples_ReturnsValidCompletionOrder(
        int numCourses, int[][] prerequisites, bool expectedSolvable) =>
        AssertValidOrder(
            numCourses,
            prerequisites,
            expectedSolvable,
            CourseScheduleIISolution.FindOrderByKahnsTopologicalSort(numCourses, prerequisites));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindOrderByNaiveRescan_LeetCodeExamples_ReturnsValidCompletionOrder(
        int numCourses, int[][] prerequisites, bool expectedSolvable) =>
        AssertValidOrder(
            numCourses,
            prerequisites,
            expectedSolvable,
            CourseScheduleIISolution.FindOrderByNaiveRescan(numCourses, prerequisites));

    private static void AssertValidOrder(
        int numCourses, int[][] prerequisites, bool expectedSolvable, int[] order)
    {
        if (!expectedSolvable)
        {
            Assert.Empty(order);
            return;
        }

        Assert.Equal(numCourses, order.Length);
        Assert.Equal(Enumerable.Range(0, numCourses).ToHashSet(), order.ToHashSet());

        foreach (var prerequisite in prerequisites)
        {
            var dependent = prerequisite[0];
            var required = prerequisite[1];

            Assert.True(
                Array.IndexOf(order, required) < Array.IndexOf(order, dependent),
                $"course {required} must come before course {dependent} in {string.Join(",", order)}");
        }
    }
}
