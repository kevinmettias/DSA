namespace DSAExperimentation.LeetCode.CourseScheduleII;

// Edges point prerequisite -> dependent course, matching Kahn's algorithm's own
// in-degree bookkeeping: a course's in-degree is its remaining unresolved
// prerequisite count. Answers LC 210 alone - a plain adjacency-list graph node
// with no fixed vertex set or modulus, so it lives beside the solution rather
// than in Domain/ (ARCHITECTURE.md #17.6). Mirrors LeetCode/CourseSchedule's own
// CourseNode (LC 207 asks only whether completion is possible; LC 210 wants the
// actual order Kahn's algorithm already produces along the way) - kept as a
// separate copy rather than shared because each LeetCode problem folder is
// self-contained (ARCHITECTURE.md #17.2).
internal sealed class CourseNode(int id)
{
    public int Id { get; } = id;

    public List<CourseNode> EnabledCourses { get; } = [];

    public override string ToString() => Id.ToString();
}
