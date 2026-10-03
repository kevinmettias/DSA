using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

// Edges point prerequisite -> dependent course, matching Kahn's algorithm's own in-degree
// bookkeeping: a course's in-degree is its remaining unresolved prerequisite count.
using CourseNode = DSAExperimentation.DataStructures.Graph.Adjacency.AdjacencyNode;
using CourseTopology = DSAExperimentation.DataStructures.Graph.Adjacency.AdjacencyTopology;

namespace DSAExperimentation.LeetCode.CourseSchedule;

// LeetCode 207. Course Schedule: can every course be completed given
// prerequisite pairs? Exactly Kahn's algorithm's own "does a full ordering
// exist" question - TopologicalSort.TrySort returns false precisely when a
// cycle (courses that depend on each other) makes completion impossible.
internal static class CourseScheduleSolution
{
    // The textbook arm the topological sort is measured against: colour each course
    // and treat an edge back into a course still on the current path as the cycle.
    // Same verdict as Kahn's algorithm, reached by depth-first search over the same
    // prerequisite edges rather than by draining in-degrees.
    public static bool CanFinishByDepthFirstColoring(int numCourses, int[][] prerequisites)
    {
        var dependents = new List<int>[numCourses];

        for (var course = 0; course < numCourses; course++)
        {
            dependents[course] = [];
        }

        foreach (var prerequisite in prerequisites)
        {
            dependents[prerequisite[1]].Add(prerequisite[0]);
        }

        var colours = new CourseColour[numCourses];

        for (var course = 0; course < numCourses; course++)
        {
            if (colours[course] == CourseColour.Unvisited && HasCycle(course, dependents, colours))
            {
                return false;
            }
        }

        return true;
    }

    private static bool HasCycle(int course, List<int>[] dependents, CourseColour[] colours)
    {
        colours[course] = CourseColour.OnCurrentPath;

        foreach (var dependent in dependents[course])
        {
            if (colours[dependent] == CourseColour.OnCurrentPath)
            {
                return true;
            }

            if (colours[dependent] == CourseColour.Unvisited && HasCycle(dependent, dependents, colours))
            {
                return true;
            }
        }

        colours[course] = CourseColour.Settled;
        return false;
    }

    private enum CourseColour
    {
        Unvisited,
        OnCurrentPath,
        Settled,
    }

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
            courses[required].Neighbors.Add(courses[dependent]);
        }

        return TopologicalSort.TrySort<
            CourseNode, CourseTopology, ListChildren<CourseNode>,
            NaturalChildOrder<CourseNode, ListChildren<CourseNode>>, ListChildren<CourseNode>>(
            courses, out _);
    }
}
