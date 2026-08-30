using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.CourseScheduleII.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CourseScheduleII;

// LeetCode 210. Course Schedule II: the same Kahn's-algorithm question as Course
// Schedule (LC 207), except the caller now wants the actual ordering
// TopologicalSort.TrySort already produces via its own `out` parameter, not just
// whether one exists - an empty array signals a cycle, matching TrySort's
// false-on-cycle result one-for-one.
public sealed partial class CourseScheduleIITests
{
    [Fact]
    public void FindOrder_NoCycle_ReturnsPrerequisitesFirst()
    {
        var course0 = new CourseNode(0);
        var course1 = new CourseNode(1);
        course0.EnabledCourses.Add(course1);

        var order = FindOrder([course0, course1]);

        Assert.Equal([0, 1], order);
    }

    [Fact]
    public void FindOrder_CircularPrerequisites_ReturnsEmptyOrder()
    {
        var course0 = new CourseNode(0);
        var course1 = new CourseNode(1);
        course0.EnabledCourses.Add(course1);
        course1.EnabledCourses.Add(course0);

        var order = FindOrder([course0, course1]);

        Assert.Empty(order);
    }

    [Fact]
    public void FindOrder_DiamondDependency_KeepsEachPrerequisiteBeforeItsDependents()
    {
        var course0 = new CourseNode(0);
        var course1 = new CourseNode(1);
        var course2 = new CourseNode(2);
        var course3 = new CourseNode(3);
        course0.EnabledCourses.Add(course1);
        course0.EnabledCourses.Add(course2);
        course1.EnabledCourses.Add(course3);
        course2.EnabledCourses.Add(course3);

        var order = FindOrder([course0, course1, course2, course3]);

        Assert.Equal(4, order.Length);
        Assert.True(Array.IndexOf(order, 0) < Array.IndexOf(order, 1));
        Assert.True(Array.IndexOf(order, 0) < Array.IndexOf(order, 2));
        Assert.True(Array.IndexOf(order, 1) < Array.IndexOf(order, 3));
        Assert.True(Array.IndexOf(order, 2) < Array.IndexOf(order, 3));
    }

    private static int[] FindOrder(List<CourseNode> courses)
    {
        var canFinish = TopologicalSort.TrySort<
            CourseNode, CourseTopology, ListChildren<CourseNode>,
            NaturalChildOrder<CourseNode, ListChildren<CourseNode>>, ListChildren<CourseNode>>(
            courses, out var ordering);

        return canFinish ? ordering.Select(c => c.Id).ToArray() : [];
    }
}
