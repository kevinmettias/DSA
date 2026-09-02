using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.CourseSchedule;

// LeetCode 207. Course Schedule: can every course be completed given
// prerequisite pairs? Exactly Kahn's algorithm's own "does a full ordering
// exist" question - TopologicalSort.TrySort returns false precisely when a
// cycle (courses that depend on each other) makes completion impossible.
internal static class CourseScheduleSolution
{
    // LeetCode's own input shape: prerequisites[i] = [a, b] means course b must
    // be completed before course a, i.e. an edge from b (prerequisite) to a
    // (dependent).
    public static bool CanFinishByTopologicalSort(int numCourses, int[][] prerequisites)
    {
        var courses = new CourseNode[numCourses];

        for (var id = 0; id < numCourses; id++)
        {
            courses[id] = new CourseNode(id);
        }

        foreach (var prerequisite in prerequisites)
        {
            var dependent = prerequisite[0];
            var required = prerequisite[1];
            courses[required].EnabledCourses.Add(courses[dependent]);
        }

        return TopologicalSort.TrySort<
            CourseNode, CourseTopology, ListChildren<CourseNode>,
            NaturalChildOrder<CourseNode, ListChildren<CourseNode>>, ListChildren<CourseNode>>(
            courses, out _);
    }
}
