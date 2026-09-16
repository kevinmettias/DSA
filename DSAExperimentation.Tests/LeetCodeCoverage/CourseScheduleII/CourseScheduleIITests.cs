using DSAExperimentation.LeetCode.CourseScheduleII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CourseScheduleII;

// Harness only. Both strategies are CourseScheduleIISolution's - this file pins
// them to LeetCode's published examples, stated in LeetCode's own (numCourses,
// prerequisites) input shape. Multiple orderings can satisfy the same
// prerequisites (the diamond-dependency example accepts both [0,1,2,3] and
// [0,2,1,3]), so a returned order is checked against the prerequisite
// constraints themselves rather than against one fixed expected array.
public sealed partial class CourseScheduleIITests
{
    public static TheoryData<CourseOrderExample> Examples =>
        new()
        {
            { new CourseOrderExample(NumCourses: 2, Prerequisites: [[1, 0]], Solvable: true) },
            { new CourseOrderExample(NumCourses: 2, Prerequisites: [[1, 0], [0, 1]], Solvable: false) },
            { new CourseOrderExample(NumCourses: 4, Prerequisites: [[1, 0], [2, 0], [3, 1], [3, 2]], Solvable: true) },
            { new CourseOrderExample(NumCourses: 1, Prerequisites: [], Solvable: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindOrderByKahnsTopologicalSort_LeetCodeExamples_ReturnsValidCompletionOrder(
        CourseOrderExample example)
    {
        var order = CourseScheduleIISolution.FindOrderByKahnsTopologicalSort(
            example.NumCourses, example.Prerequisites);

        AssertValidOrder(example, order);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindOrderByNaiveRescan_LeetCodeExamples_ReturnsValidCompletionOrder(
        CourseOrderExample example)
    {
        var order = CourseScheduleIISolution.FindOrderByNaiveRescan(example.NumCourses, example.Prerequisites);

        AssertValidOrder(example, order);
    }

    private static void AssertValidOrder(CourseOrderExample example, int[] order)
    {
        if (!example.Solvable)
        {
            Assert.Empty(order);
            return;
        }

        Assert.Equal(example.NumCourses, order.Length);
        Assert.Equal(Enumerable.Range(0, example.NumCourses).ToHashSet(), order.ToHashSet());

        foreach (var prerequisite in example.Prerequisites)
        {
            var dependent = prerequisite[0];
            var required = prerequisite[1];

            Assert.True(
                Array.IndexOf(order, required) < Array.IndexOf(order, dependent),
                $"course {required} must come before course {dependent} in {string.Join(",", order)}");
        }
    }

    // One LeetCode example: the course count, the prerequisite pairs, and whether a
    // completion order exists at all. The solvable flag is named at the row that states
    // it, so a reader of `Examples` never has to remember which position `true` sits in.
    public readonly record struct CourseOrderExample(int NumCourses, int[][] Prerequisites, bool Solvable);
}
