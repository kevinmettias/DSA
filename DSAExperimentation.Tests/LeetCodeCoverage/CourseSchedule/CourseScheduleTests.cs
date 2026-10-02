using DSAExperimentation.LeetCode.CourseSchedule;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CourseSchedule;

// Harness only. The single strategy is CourseScheduleSolution's - this file
// checks LeetCode's published examples, stated in LeetCode's own
// (numCourses, prerequisites) input shape.
public sealed partial class CourseScheduleTests
{
    public static TheoryData<CourseExample> Examples =>
        new()
        {
            { new CourseExample(NumCourses: 2, Prerequisites: [[1, 0]], Expected: true) },
            { new CourseExample(NumCourses: 2, Prerequisites: [[1, 0], [0, 1]], Expected: false) },
            { new CourseExample(NumCourses: 1, Prerequisites: [], Expected: true) },
            { new CourseExample(NumCourses: 4, Prerequisites: [[1, 0], [2, 0], [3, 1], [3, 2]], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanFinishByTopologicalSort_LeetCodeExamples_ReturnsWhetherCompletionIsPossible(
        CourseExample example)
    {
        var canFinish = CourseScheduleSolution.CanFinishByTopologicalSort(example.NumCourses, example.Prerequisites);

        Assert.Equal(example.Expected, canFinish);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanFinishByDepthFirstColoring_LeetCodeExamples_ReturnsWhetherCompletionIsPossible(
        CourseExample example)
    {
        var canFinish = CourseScheduleSolution.CanFinishByDepthFirstColoring(example.NumCourses, example.Prerequisites);

        Assert.Equal(example.Expected, canFinish);
    }

    // The two arms are competing strategies for one question, so they must agree on the cyclic
    // example as well as the acyclic ones - a colouring that never finds the cycle would pass
    // every other case here.
    [Theory]
    [MemberData(nameof(Examples))]
    public void CanFinish_AgreeOnEveryExample(CourseExample example) =>
        Assert.Equal(
            CourseScheduleSolution.CanFinishByTopologicalSort(example.NumCourses, example.Prerequisites),
            CourseScheduleSolution.CanFinishByDepthFirstColoring(example.NumCourses, example.Prerequisites));

    // One LeetCode example: the course count, the prerequisite pairs in LeetCode's own
    // [a, b] = "take b before a" shape, and whether every course can be finished. Nested
    // because it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct CourseExample(int NumCourses, int[][] Prerequisites, bool Expected);
}
