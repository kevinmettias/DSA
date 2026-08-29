using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.CourseSchedule.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CourseSchedule;

// LeetCode 207. Course Schedule: can every course be completed given prerequisite
// pairs? Exactly Kahn's algorithm's own "does a full ordering exist" question -
// TopologicalSort.TrySort returns false precisely when a cycle (courses that
// depend on each other) makes completion impossible.
public sealed partial class CourseScheduleTests
{
    [Fact]
    public void CanFinish_NoCycle_ReturnsTrueWithPrerequisitesFirst()
    {
        var course0 = new CourseNode(0);
        var course1 = new CourseNode(1);
        course0.EnabledCourses.Add(course1);

        var canFinish = TrySort([course0, course1], out var ordering);

        Assert.True(canFinish);
        Assert.Equal([course0, course1], ordering);
    }

    [Fact]
    public void CanFinish_CircularPrerequisites_ReturnsFalse()
    {
        var course0 = new CourseNode(0);
        var course1 = new CourseNode(1);
        course0.EnabledCourses.Add(course1);
        course1.EnabledCourses.Add(course0);

        var canFinish = TrySort([course0, course1], out _);

        Assert.False(canFinish);
    }

    private static bool TrySort(List<CourseNode> courses, out List<CourseNode> ordering)
        => TopologicalSort.TrySort<
            CourseNode, CourseTopology, ListChildren<CourseNode>,
            NaturalChildOrder<CourseNode, ListChildren<CourseNode>>, ListChildren<CourseNode>>(
            courses, out ordering);
}
