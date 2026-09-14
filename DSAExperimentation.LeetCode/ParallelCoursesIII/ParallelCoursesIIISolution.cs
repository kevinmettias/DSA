using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.ParallelCoursesIII;

// LeetCode 2050. Parallel Courses III: courses may run in parallel, each takes its
// own number of months, and a course can only start once every prerequisite has
// finished - so the answer is the critical-path length, the longest
// duration-weighted path through the prerequisite DAG.
//
// Both strategies run the same max-finish-time DP - finish[node] is the earliest
// month node can complete on any prerequisite chain ending at it - and differ only
// in how they order the relaxations. KahnsTopologicalSortDp composes this repo's
// own TopologicalSort.TrySort for a dependency-respecting order in which every edge
// is relaxed exactly once, O(V+E) - the same "push a running value onto every child,
// relaxed once instead of iterated to a fixed point" shape
// LargestColorValueInADirectedGraph already uses, just relaxing a finish time
// instead of a per-color count. RepeatedRelaxation is the textbook answer when you
// have no ordering to hand: relax every edge, once per node, for as many rounds as
// there are nodes (Bellman-Ford style), which converges regardless of processing
// order at O(V*E).
//
// Courses form a guaranteed-acyclic DAG per LC's own constraint, so the cycle case
// TrySort's bool return would signal never fires here.
internal static class ParallelCoursesIIISolution
{
    // LeetCode's own input shape: courses are 1-indexed, relations[j] =
    // [prevCourse, nextCourse] means prevCourse must finish before nextCourse may
    // start, and time[i] is course i + 1's duration in months.
    public static int MinimumTimeByKahnsTopologicalSortDp(int n, int[][] relations, int[] time) =>
        MinimumTimeByKahnsTopologicalSortDp(BuildCourses(n, relations, time));

    public static int MinimumTimeByKahnsTopologicalSortDp(List<CourseTimeNode> courses)
    {
        TopologicalSort.TrySort<
            CourseTimeNode, CourseTimeTopology, ListChildren<CourseTimeNode>,
            NaturalChildOrder<CourseTimeNode, ListChildren<CourseTimeNode>>, ListChildren<CourseTimeNode>>(
            courses, out var ordering);

        var readyAt = courses.ToDictionary(course => course, _ => 0);
        var best = 0;

        foreach (var course in ordering)
        {
            best = Math.Max(best, RelaxForward(course, readyAt));
        }

        return best;
    }

    // Finishes course at the earliest month every prerequisite allows, then pushes
    // that finish onto each dependent's start (Kahn's order guarantees course is
    // fully finalized before any dependent is visited). Returns course's own finish.
    private static int RelaxForward(CourseTimeNode course, Dictionary<CourseTimeNode, int> readyAt)
    {
        var finish = readyAt[course] + course.Time;
        var children = CourseTimeTopology.GetChildren(course);

        for (var i = 0; i < children.Count; i++)
        {
            var child = children.Get(i);
            readyAt[child] = Math.Max(readyAt[child], finish);
        }

        return finish;
    }

    // The textbook answer: relax every edge of every node for as many rounds as
    // there are nodes, which reaches the fixed point whatever order the nodes happen
    // to be enumerated in. Deliberately written with BCL collections and no ordering
    // primitive - it is the arm the composed solution above has to justify itself
    // against.
    public static int MinimumTimeByRepeatedRelaxation(int n, int[][] relations, int[] time) =>
        MinimumTimeByRepeatedRelaxation(BuildCourses(n, relations, time));

    public static int MinimumTimeByRepeatedRelaxation(List<CourseTimeNode> courses)
    {
        var finish = courses.ToDictionary(course => course, course => course.Time);

        for (var round = 0; round < courses.Count; round++)
        {
            foreach (var course in courses)
            {
                RelaxSuccessors(course, finish);
            }
        }

        return finish.Values.Max();
    }

    private static void RelaxSuccessors(CourseTimeNode course, Dictionary<CourseTimeNode, int> finish)
    {
        var courseFinish = finish[course];

        foreach (var child in course.Successors)
        {
            var candidate = courseFinish + child.Time;

            if (candidate > finish[child])
            {
                finish[child] = candidate;
            }
        }
    }

    private static List<CourseTimeNode> BuildCourses(int n, int[][] relations, int[] time)
    {
        var courses = Enumerable.Range(0, n).Select(id => new CourseTimeNode(id, time[id])).ToList();

        foreach (var relation in relations)
        {
            courses[relation[0] - 1].Successors.Add(courses[relation[1] - 1]);
        }

        return courses;
    }
}
