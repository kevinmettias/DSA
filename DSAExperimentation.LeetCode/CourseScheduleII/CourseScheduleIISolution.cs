using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.CourseScheduleII;

// LeetCode 210. Course Schedule II: the same Kahn's-algorithm question as Course
// Schedule (LC 207), except the caller now wants the actual ordering
// TopologicalSort.TrySort already produces via its own `out` parameter, not just
// whether one exists - an empty array signals a cycle, matching TrySort's
// false-on-cycle result one-for-one.
//
// The naive baseline rescans every remaining course for one with zero remaining
// prerequisites on each step, O(V^2 + V*E); TopologicalSort.TrySort is Kahn's
// algorithm proper, O(V+E) via a queue of already-zero-in-degree courses.
internal static class CourseScheduleIISolution
{
    // LeetCode's own input shape: prerequisites[i] = [a, b] means course b must
    // be completed before course a, i.e. an edge from b (prerequisite) to a
    // (dependent).
    public static int[] FindOrderByKahnsTopologicalSort(int numCourses, int[][] prerequisites) =>
        FindOrderByKahnsTopologicalSort(BuildCourses(numCourses, prerequisites));

    public static int[] FindOrderByKahnsTopologicalSort(List<CourseNode> courses)
    {
        var canFinish = TopologicalSort.TrySort<
            CourseNode, CourseTopology, ListChildren<CourseNode>,
            NaturalChildOrder<CourseNode, ListChildren<CourseNode>>, ListChildren<CourseNode>>(
            courses, out var ordering);

        return canFinish ? ordering.Select(c => c.Id).ToArray() : [];
    }

    // The textbook answer: rescan the remaining courses from scratch on every
    // step looking for the next one with no unresolved prerequisites, rather
    // than tracking a frontier queue. Deliberately written without this repo's
    // TopologicalSort - it is the arm the composed solution above has to
    // justify itself against.
    public static int[] FindOrderByNaiveRescan(int numCourses, int[][] prerequisites) =>
        FindOrderByNaiveRescan(BuildCourses(numCourses, prerequisites));

    public static int[] FindOrderByNaiveRescan(List<CourseNode> courses)
    {
        var inDegree = courses.ToDictionary(course => course, _ => 0);
        foreach (var course in courses)
        {
            foreach (var next in course.EnabledCourses)
            {
                inDegree[next]++;
            }
        }

        var remaining = new List<CourseNode>(courses);
        var order = new List<CourseNode>(courses.Count);

        while (remaining.Count > 0 && TryAdvanceNaiveRescan(remaining, inDegree, order))
        {
        }

        return order.Count == courses.Count ? order.Select(c => c.Id).ToArray() : [];
    }

    private static bool TryAdvanceNaiveRescan(
        List<CourseNode> remaining, Dictionary<CourseNode, int> inDegree, List<CourseNode> order)
    {
        var next = remaining.FirstOrDefault(course => inDegree[course] == 0);

        if (next is null)
        {
            return false;
        }

        order.Add(next);
        remaining.Remove(next);

        foreach (var dependent in next.EnabledCourses)
        {
            inDegree[dependent]--;
        }

        return true;
    }

    private static List<CourseNode> BuildCourses(int numCourses, int[][] prerequisites)
    {
        var courses = new List<CourseNode>(numCourses);

        for (var id = 0; id < numCourses; id++)
        {
            courses.Add(new CourseNode(id));
        }

        foreach (var prerequisite in prerequisites)
        {
            var dependent = prerequisite[0];
            var required = prerequisite[1];
            courses[required].EnabledCourses.Add(courses[dependent]);
        }

        return courses;
    }
}
