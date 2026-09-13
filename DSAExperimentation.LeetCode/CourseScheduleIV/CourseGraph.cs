namespace DSAExperimentation.LeetCode.CourseScheduleIV;

// The whole prerequisite network, built once from LeetCode's own numCourses +
// prerequisites pair - the model, not an answer to any one query about it. Both
// strategies take one of these, so a benchmark can charge construction to
// [GlobalSetup] instead of to the reachability work being measured
// (ARCHITECTURE.md 17.4).
internal sealed class CourseGraph
{
    // Every prerequisite relation costs the same; the shortest-path primitive
    // this graph is handed to needs *a* weight, and the answers it feeds only
    // ever ask whether a pair is connected at all.
    private const int PrerequisiteWeight = 1;

    // Indexed by course id, so Courses[i].Id == i.
    public CourseNode[] Courses { get; }

    private CourseGraph(CourseNode[] courses) => Courses = courses;

    // LeetCode's own input shape: prerequisites[i] = [a, b] means course a must
    // be taken before course b, i.e. an edge from a (prerequisite) to b
    // (dependent).
    public static CourseGraph Build(int numCourses, int[][] prerequisites)
    {
        var courses = new CourseNode[numCourses];

        for (var id = 0; id < numCourses; id++)
        {
            courses[id] = new CourseNode(id);
        }

        foreach (var prerequisite in prerequisites)
        {
            var (required, dependent) = (prerequisite[0], prerequisite[1]);
            courses[required].Edges.Add((PrerequisiteWeight, courses[dependent]));
        }

        return new CourseGraph(courses);
    }
}
